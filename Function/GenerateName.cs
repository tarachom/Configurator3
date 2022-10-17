using AccountingSoftware;

namespace Configurator
{
    class GenerateName
    {
        public static string GetNewName()
        {
            string[] EnglishAlphabet = Configuration.GetEnglishAlphabet();

            string random = "";

            for (int i = 0; i < 5; i++)
                random += EnglishAlphabet[new Random().Next(EnglishAlphabet.Length)];

            return "_" + random;
        }
    }
}