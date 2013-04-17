using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.NHibernate.Hql
{
    internal class HqlGenerator
    {
        /// <summary>
        /// Genera una query per ottenere un' entità filtrando per constraints
        /// E' possibile usare anche contraints relativi a relazioni se specificati nelle associations
        /// </summary>
        public static string GetEntitiesQuery<TEntity>(string costraints, string associations)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(costraints))
            {
                sb.Append(string.Format("select {0} from {0} as {0} {1} where ", typeof(TEntity).Name, associations));
                sb.Append(costraints);
            }
            else sb.Append(string.Format("select {0} from {0} as {0} {1} ", typeof(TEntity).Name, associations));
            return sb.ToString();
        }

        /// <summary>
        /// Genera una query per ottenere un' entità precaricando con una join le relazioni figlie, filtrando per constraints
        /// E' possibile usare anche contraints relativi a relazioni se specificati nelle associations
        /// </summary>
        public static string GetFetchEntitiesQuery<TEntity>(string costraints, string associations, IEnumerable<string> fetchPropertyEntities)
        {

            var sb = new StringBuilder();
            string entityName = typeof(TEntity).Name;
            if (!string.IsNullOrEmpty(costraints))
            {
                sb.Append(string.Format("select {0} from {0} as {0} {1} {2} where ", entityName, associations, FetchBuildProperties(entityName, fetchPropertyEntities)));
                sb.Append(costraints);
            }
            else sb.Append(string.Format("select {0} from {0} as {0} {1} {2}", entityName, associations, FetchBuildProperties(entityName, fetchPropertyEntities)));
            return sb.ToString();
        }

        /// <summary>
        /// Genera una query per tornare le proiezioni di una o piu entità
        /// </summary>
        public static string GetProjectionQuery<TEntity>(IEnumerable<IProjection> projections, IEnumerable<IRestriction> restrictions, IEnumerable<IJoin> joins, IEnumerable<IOrdination> ordinations = null, bool appendFirstEntityId = true)
        {
            #region comments
            //"select InjectionProcedure.Id as Id, InjectionProcedure.EndTime as EndTime, Patient.GivenName as Name, 
            //ProgrammedContrastPhases.Id as PhaseIds from InjectionProcedure as InjectionProcedure
            //left join InjectionProcedure.InjectionProtocols as InjectionProtocols
            //left join InjectionProcedure.Patient as Patient left join InjectionProcedure.Contrast as Contrast
            //left join InjectionProtocols.ActualContrastPhases as ActualContrastPhases
            //left join InjectionProtocols.ProgrammedContrastPhases as ProgrammedContrastPhases
            //where (year(InjectionProcedure.EndTime) = '2011' )";


            // versione minimale
            //"select InjectionProcedure.Id as Id,
            //InjectionProcedure.EndTime as EndTime,
            //Patient.GivenName as Name,
            //ProgrammedContrastPhases.Id as PhaseIds 
            //from InjectionProcedure as InjectionProcedure 
            //left join InjectionProcedure.Patient as Patient 
            //left join InjectionProcedure.InjectionProtocols as InjectionProtocols 
            //left join InjectionProtocols.ProgrammedContrastPhases as ProgrammedContrastPhases 
            //where (year(InjectionProcedure.EndTime) = '2011' )";

            //projection("Id","Id")
            //projection("EndTime","EndTime")
            //projection("Patient.GivenName","Name")
            //projection("InjectionProtocols.ProgrammedContrastPhases.Id","PhaseIds")

            // required joins 
            // Patient : InjectionProcedure.Patient as Patient 
            // InjectionProtocols : InjectionProcedure.InjectionProtocols as InjectionProtocols 
            // ProgrammedContrastPhases : InjectionProtocols.ProgrammedContrastPhases as as ProgrammedContrastPhases  
            #endregion
            
            ProjectionCheckProjectionNameIfIsValid(projections);
            
            // merge required joins
            List<IJoin> allJoins = new List<IJoin>();
            allJoins.AddRange((from projection in projections select projection.Join).ToList());
            
            if (joins != null)
                allJoins.AddRange(joins);

            // merge required joins from restrictions
            List<IJoin> restrictionJoins = new List<IJoin>();
            restrictionJoins.AddRange((from restrictionsJoins in restrictions select restrictionsJoins.Join).ToList());

            if(restrictionJoins.Any())
                allJoins.AddRange(restrictionJoins);
            
            List<IGroupBy> allGroupsBy = new List<IGroupBy>();
            allGroupsBy.AddRange((from projection in projections select projection.GroupBy).ToList());
           
            var entityName = typeof(TEntity).Name;            
            var sb = new StringBuilder();
            sb.Append("select ");
            ProjectionBuildProjectionsSelects(ref sb, entityName, projections, appendFirstEntityId);
            sb.Append(String.Format(" from {0} as {0} ", entityName));
            ProjectionBuildLeftJoins(ref sb, allJoins);
            ProjectionBuildWhere(ref sb, restrictions);
            ProjectionBuildGroupBy(ref sb, entityName, allGroupsBy, appendFirstEntityId);
            ProjectionBuildOrderBy(ref sb, entityName, ordinations);
            return sb.ToString();
        }

        private static void ProjectionBuildLeftJoins(ref StringBuilder sb, IEnumerable<IJoin> joins)
        {
            IEnumerable<IJoin> orderedJoinsByPriority = joins.OrderBy(join => join.PriorityJoins);
            IList<string> requiredJoins = new List<string>();
            foreach (var projection in orderedJoinsByPriority)
            {
                foreach (var requiredJoin in projection.GetJoin())
                {
                    if (!requiredJoins.Contains(requiredJoin))
                        requiredJoins.Add(requiredJoin);
                }
            }
            foreach (var requiredJoin in requiredJoins)
            {
                sb.Append(requiredJoin);
                sb.Append(" ");
            }
        }

        private static void ProjectionBuildProjectionsSelects(ref StringBuilder sb, string entityName, IEnumerable<IProjection> projections, bool appendFirstEntityId = true)
        {
            var count = projections.Count();
            
            if (appendFirstEntityId)
                sb.Append(String.Format("{0}.Id as Id, ", entityName));
            
            for (int i = 0; i < count; i++)
            {
                sb.Append(projections.ElementAt(i).ProjectionName);

                if (projections.ElementAt(i).ProjectionAlias != null)
                {
                    sb.Append(" as ");
                    sb.Append(projections.ElementAt(i).ProjectionAlias);
                }

                if (i != count - 1)
                    sb.Append(", ");
            }
        }

        private static void ProjectionCheckProjectionNameIfIsValid(IEnumerable<IProjection> projections)
        {
            if (projections.Any(projection => projection.ProjectionAlias != null && projection.ProjectionAlias.Equals("Id", StringComparison.CurrentCultureIgnoreCase)))
                throw new Exception("Projections cannot have an alias named [Id]. Please use a different alias ");
        }

        private static void ProjectionBuildWhere(ref StringBuilder sb, IEnumerable<IRestriction> restrictions)
        {
            var restrictionLenght = restrictions.Count();
            if (restrictionLenght > 0)
            {
                sb.Append(" where ");
                for (int i = 0; i < restrictionLenght; i++)
                {
                    sb.Append(String.Concat(restrictions.ElementAt(i).GetRestriction()));
                    if (i != restrictionLenght - 1)
                        sb.Append(" and ");
                }
            }
        }

        private static void ProjectionBuildGroupBy(ref StringBuilder sb, string entityName, IEnumerable<IGroupBy> groupBys, bool appendFirstEntityId)
        {
            if (groupBys == null) return;

            var containStopgroupBy = groupBys.Count(x => x.StopGroupBy);
            if (containStopgroupBy == 0) return;

            var groupsLenght = groupBys.Count();
            if (groupsLenght > 0)
            {
                sb.Append(" group by ");

                if (appendFirstEntityId)
                    sb.Append(String.Format("{0}.Id ", entityName));

                for (int i = 0; i < groupsLenght; i++)
                {
                    if (groupBys.ElementAt(i).StopGroupBy)
                        break;
                    if ((i < groupsLenght - 1) && !(i == 0 && !appendFirstEntityId))
                        sb.Append(" , ");

                    sb.Append(String.Concat(groupBys.ElementAt(i).GetGroupBy()));
                }
            }
        }

        private static void ProjectionBuildOrderBy(ref StringBuilder sb, string entityName, IEnumerable<IOrdination> ordinations)
        {
            if (ordinations == null) return;
            var groupsLenght = ordinations.Count();
            
            if (groupsLenght > 0)
            {
                sb.Append(" order by ");
                
                for (int i = 0; i < groupsLenght; i++)
                {
                    var ordinationItem = string.Format("{0}.{1}", entityName, ordinations.ElementAt(i).GetOrdination());

                    sb.Append(ordinationItem);
                    if (i < groupsLenght - 1)
                        sb.Append(", ");
                }
            }
        }

        private static string FetchBuildProperties(string entityName, IEnumerable<string> fetchPropertyEntities)
        {
            var sb = new StringBuilder();
            foreach (var fetchPropertyEntity in fetchPropertyEntities)
            {
                if (fetchPropertyEntity.Contains("."))
                    sb.Append(string.Format(" left join fetch {0}", fetchPropertyEntity));
                else sb.Append(string.Format(" left join fetch {0}.{1} as {1}", entityName, fetchPropertyEntity));
            }
            return sb.ToString();
        }
    }
}
