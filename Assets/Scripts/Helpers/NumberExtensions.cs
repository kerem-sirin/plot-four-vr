using System.Collections.Generic;

namespace PlotFourVR.Helpers
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Turns 1 to “One”, 42 to “Forty Two”, etc.
        /// </summary>
        public static string ToWords(int n)
        {
            {
                if (n == 0) return "Zero";
                if (n < 0) return "Minus " + ToWords(-n);

                var parts = new List<string>();
                void AddChunk(int value, string singular, string plural)
                {
                    if (value == 0) return;
                    var text = ToWords(value);
                    parts.Add(text + " " + (value == 1 ? singular : plural));
                }

                if (n >= 1_000_000_000) { AddChunk(n / 1_000_000_000, "Billion", "Billion"); n %= 1_000_000_000; }
                if (n >= 1_000_000) { AddChunk(n / 1_000_000, "Million", "Million"); n %= 1_000_000; }
                if (n >= 1000) { AddChunk(n / 1000, "Thousand", "Thousand"); n %= 1000; }
                if (n >= 100) { AddChunk(n / 100, "Hundred", "Hundred"); n %= 100; }

                var tensMap = new[] {
                "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
            };
                var onesMap = new[] {
                "Zero","One","Two","Three","Four","Five","Six","Seven","Eight","Nine",
                "Ten","Eleven","Twelve","Thirteen","Fourteen","Fifteen","Sixteen",
                "Seventeen","Eighteen","Nineteen"
            };

                if (n >= 20)
                {
                    parts.Add(tensMap[n / 10]);
                    n %= 10;
                }
                if (n > 0)
                {
                    parts.Add(onesMap[n]);
                }

                return string.Join(" ", parts);
            }
        }
    }
}