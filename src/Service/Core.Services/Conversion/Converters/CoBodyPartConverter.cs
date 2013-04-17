using System;

namespace Core.Services.Conversion
{
    public class CoBodyPartConverter : IConvertCommand
    {
        public object ConvertToDto(object toConvert, Type requestedType)
        {
            switch (Convert.ToInt32(toConvert))
            {
                case 0:
                    return "undefined";
                case 1:
                    return "head";
                case 2:
                    return "neck";
                case 3:
                    return "chest";
                case 4:
                    return "abdomen";
                case 5:
                    return "arm";
                case 6:
                    return "hand";
                case 7:
                    return "thigh";
                case 8:
                    return "calf";
                case 9:
                    return "foot";
                case 10:
                    return "cardiac";
                case 11:
                    return "showall";
                default:
                    throw new ConvertCommandException(typeof(CoBodyPartConverter), toConvert);
            }

        }

        public object ConvertToEntity(object toConvert, Type requestedType)
        {
            switch (toConvert.ToString().ToLower())
            {
                case "undefined":
                    return 0;
                case "head":
                    return 1;
                case "neck":
                    return 2;
                case "chest":
                    return 3;
                case "abdomen":
                    return 4;
                case "arm":
                    return 5;
                case "hand":
                    return 6;
                case "thigh":
                    return 7;
                case "calf":
                    return 8;
                case "foot":
                    return 9;
                case "cardiac":
                    return 10;
                case "showall":
                    return 11;
                default:
                    throw new ConvertCommandException(typeof(CoBodyPartConverter), toConvert);
            }
        }
    }
}
