using System;
using System.Collections.Generic;

namespace Services.Units
{
    /// <summary>
    /// Catálogo de unidades e medidas caseiras.
    /// Observação: conversões ml->g usam densidade padrão 1 (água).
    /// Para ficar perfeito por alimento líquido, adicione densidade por food.
    /// </summary>
    public static class UnitCatalog
    {
        public sealed record UnitDef(string Key, string Label, double? GramsPerUnit, double? MlPerUnit);

        // Chaves estáveis para o front/back (não traduzir a Key).
        public static readonly IReadOnlyList<UnitDef> All = new List<UnitDef>
        {
            // Massa
            new("g", "Gramas", 1.0, null),
            new("kg", "Quilos", 1000.0, null),

            // Volume
            new("ml", "Mililitros", null, 1.0),
            new("l", "Litros", null, 1000.0),

            // Unidade
            // Regra de compatibilidade desta base: 1 unidade = 100g.
            // Isso evita travar o fluxo nas refeições quando o front envia `un`.
            // Se no futuro cada alimento tiver equivalência própria, essa regra pode ser substituída.
            new("un", "Unidade", 100.0, null),

            // Medidas caseiras (massa aproximada)
            new("tsp_level", "Colher de chá (rasa)", 2.5, null),
            new("tsp_full", "Colher de chá (cheia)", 5.0, null),
            new("tbsp_level", "Colher de sopa (rasa)", 15.0, null),
            new("tbsp_full", "Colher de sopa (cheia)", 20.0, null),
            new("rice_spoon", "Colher de arroz", 50.0, null),
            new("beans_ladle_level", "Concha de feijão (rasa)", 80.0, null),
            new("beans_ladle_full", "Concha de feijão (cheia)", 140.0, null),
            new("knife_tip", "Ponta de faca", 8.0, null),

            // Medidas caseiras (volume)
            new("americano_half", "Copo americano (metade)", null, 100.0),
            new("americano_full", "Copo americano (cheio)", null, 180.0),
            new("mug_full", "Caneca (cheia)", null, 300.0),
        };

        private static readonly Dictionary<string, UnitDef> Map = BuildMap();

        private static Dictionary<string, UnitDef> BuildMap()
        {
            var dict = new Dictionary<string, UnitDef>(StringComparer.OrdinalIgnoreCase);

            foreach (var u in All)
            {
                // Key estável (ex.: "g", "tbsp_full")
                dict[u.Key] = u;

                // Também aceita o Label (caso algum front mande texto)
                if (!string.IsNullOrWhiteSpace(u.Label))
                    dict[u.Label.Trim()] = u;
            }

            // Aliases comuns (compatibilidade com fronts antigos / inputs humanos)
            void Alias(string alias, string key)
            {
                if (dict.TryGetValue(key, out var u))
                    dict[alias] = u;
            }

            Alias("grama", "g");
            Alias("gramas", "g");
            Alias("gr", "g");
            Alias("quilo", "kg");
            Alias("quilos", "kg");
            Alias("kilograma", "kg");
            Alias("kilogramas", "kg");
            Alias("mililitro", "ml");
            Alias("mililitros", "ml");
            Alias("litro", "l");
            Alias("litros", "l");
            Alias("unidade", "un");
            Alias("unidades", "un");

            // Pequenas variações de escrita
            Alias("colher de chá rasa", "tsp_level");
            Alias("colher de chá cheia", "tsp_full");
            Alias("colher de sopa rasa", "tbsp_level");
            Alias("colher de sopa cheia", "tbsp_full");
            Alias("colher de arroz", "rice_spoon");
            Alias("concha de feijão rasa", "beans_ladle_level");
            Alias("concha de feijão cheia", "beans_ladle_full");
            Alias("ponta de faca", "knife_tip");
            Alias("copo americano metade", "americano_half");
            Alias("copo americano (metade)", "americano_half");
            Alias("copo americano cheio", "americano_full");
            Alias("caneca cheia", "mug_full");

            return dict;
        }

        public static bool TryGet(string unitKey, out UnitDef? unit)
        {
            unit = null;

            // Se vier vazio/nulo, assume gramas (compatibilidade)
            if (string.IsNullOrWhiteSpace(unitKey))
            {
                if (Map.TryGetValue("g", out var g))
                {
                    unit = g;
                    return true;
                }
                return false;
            }

            var key = unitKey.Trim();

            // Normaliza alguns formatos comuns
            key = key.Replace("\t", " ").Replace("\n", " ").Trim();

            if (Map.TryGetValue(key, out var found))
            {
                unit = found;
                return true;
            }

            // Tenta normalizar underscores/hífens (ex.: "tbsp-full", "tbsp full")
            var normalized = key.Replace("-", "_").Replace(" ", "_");
            if (Map.TryGetValue(normalized, out found))
            {
                unit = found;
                return true;
            }

            return false;
        }

        public static bool TryConvertToGrams(double quantity, string unitKey, out double grams, out string? error)
        {
            grams = 0.0;
            error = null;

            if (!TryGet(unitKey, out var unit) || unit == null)
            {
                error = "Unidade inválida.";
                return false;
            }

            if (unit.GramsPerUnit.HasValue)
            {
                grams = quantity * unit.GramsPerUnit.Value;
                return true;
            }

            // volume -> massa (densidade padrão 1g/ml)
            if (unit.MlPerUnit.HasValue)
            {
                grams = quantity * unit.MlPerUnit.Value;
                return true;
            }

            error = "Unidade não possui fator de conversão.";
            return false;
        }

        public static bool TryConvert(double quantity, string fromUnitKey, string toUnitKey, out double result, out string? error)
        {
            result = 0.0;
            error = null;

            if (!TryGet(fromUnitKey, out var from) || from == null)
            {
                error = "Unidade de origem inválida.";
                return false;
            }

            if (!TryGet(toUnitKey, out var to) || to == null)
            {
                error = "Unidade de destino inválida.";
                return false;
            }

            // massa <-> massa
            if (from.GramsPerUnit.HasValue && to.GramsPerUnit.HasValue)
            {
                var grams = quantity * from.GramsPerUnit.Value;
                result = grams / to.GramsPerUnit.Value;
                return true;
            }

            // volume <-> volume
            if (from.MlPerUnit.HasValue && to.MlPerUnit.HasValue)
            {
                var ml = quantity * from.MlPerUnit.Value;
                result = ml / to.MlPerUnit.Value;
                return true;
            }

            // massa <-> volume (densidade 1)
            if (from.GramsPerUnit.HasValue && to.MlPerUnit.HasValue)
            {
                var grams = quantity * from.GramsPerUnit.Value;
                // 1g/ml
                var ml = grams;
                result = ml / to.MlPerUnit.Value;
                return true;
            }

            if (from.MlPerUnit.HasValue && to.GramsPerUnit.HasValue)
            {
                var ml = quantity * from.MlPerUnit.Value;
                var grams = ml;
                result = grams / to.GramsPerUnit.Value;
                return true;
            }

            error = "Conversão não suportada.";
            return false;
        }
    }
}
