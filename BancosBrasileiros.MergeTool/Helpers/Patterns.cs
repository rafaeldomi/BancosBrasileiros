﻿using System.Text.RegularExpressions;

namespace BancosBrasileiros.MergeTool.Helpers
{
    internal class Patterns
    {
        
        /// <summary>
        /// The comma separated values pattern
        /// </summary>
        public static readonly Regex CsvPattern = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The semicolon separated values pattern
        /// </summary>
        public static readonly Regex SsvPattern = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The SLC pattern
        /// </summary>
        public static readonly Regex SlcPattern = new Regex(@"^(?<code>\d{1,3})\s(?<cnpj>\d{1,2}\.\d{3}\.\d{3}(?:.|\/)\d{4}([-|·|\.|\s]{1,2})\d{2})\s(?<nome>.+?)(?:[\s|X]){1,7}$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The SILOC pattern
        /// </summary>
        public static readonly Regex SilocPattern = new Regex(@"^(?<code>\d{1,3})\s(?<compe>\d{3})\s(?<ispb>\d{8})\s(?<cobranca>sim|não)\s(?<doc>sim|não)\s(?<nome>.+?)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The sitraf pattern
        /// </summary>
        public static readonly Regex SitrafPattern = new Regex(@"^(?<code>\d{1,3})\s(?<compe>\d{3})\s(?<ispb>\d{8})\s(?<nome>.+?)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The ctc pattern
        /// </summary>
        public static readonly Regex CtcPattern = new Regex(@"^(?<code>\d{1,3})\s(?<nome>.+?)\s(?<cnpj>\d{1,2}\.\d{3}\.\d{3}(?:.|\/)\d{4}([-|·|\.|\s]{1,2})\d{2})\s+(?<ispb>\d{8})\s(?<produtos>.+?)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The PCPS pattern
        /// </summary>
        public static readonly Regex PcpsPattern = new Regex(@"^(?<code>\d{1,3})\s(?<nome>.+?)\s(?<cnpj>\d{1,2}\.\d{3}\.\d{3}(?:.|\/)\d{4}([-|·|\.|\s]{1,3})\d{2})\s+(?<ispb>\d{7,8})\s(?<adesao>.+?)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
