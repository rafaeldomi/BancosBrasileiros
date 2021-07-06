﻿// ***********************************************************************
// Assembly         : BancosBrasileiros.MergeTool
// Author           : Guilherme Branco Stracini
// Created          : 19/05/2020
//
// Last Modified By : Guilherme Branco Stracini
// Last Modified On : 05-05-2021
// ***********************************************************************
// <copyright file="Seeder.cs" company="Guilherme Branco Stracini ME">
//     Copyright (c) Guilherme Branco Stracini ME. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace BancosBrasileiros.MergeTool.Helpers
{
    using CrispyWaffle.Extensions;
    using Dto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Org.BouncyCastle.Crypto.Generators;

    /// <summary>
    /// Class Seeder.
    /// </summary>
    internal class Seeder
    {
        /// <summary>
        /// The source
        /// </summary>
        private readonly IList<Bank> _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="Seeder" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentNullException">source</exception>
        public Seeder(IList<Bank> source) => _source = source ?? throw new ArgumentNullException(nameof(source));

        /// <summary>
        /// Generates the missing document.
        /// </summary>
        /// <returns>Seeder.</returns>
        public Seeder GenerateMissingDocument()
        {
            var existing = 0;
            var missing = 0;

            foreach (var bank in _source)
            {
                if (bank.Document != null && bank.Document.Length == 18)
                {
                    existing++;
                    continue;
                }

                bank.Document = bank.IspbString;
                bank.DateUpdated = DateTimeOffset.Now;
                missing++;
            }

            Console.WriteLine($"\r\nGenerate document | Existing: {existing} | Missing: {missing}\r\n");

            return this;
        }

        /// <summary>
        /// Seeds the string.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Seeder.</returns>
        public Seeder SeedStr(IEnumerable<Bank> items)
        {
            var found = 0;
            var notFound = 0;

            foreach (var str in items)
            {
                var bank = _source.SingleOrDefault(b => b.Compe == str.Compe);

                if (bank == null)
                {
                    Console.WriteLine($"Adding bank by STR List | {str.Compe} | {str.LongName}");

                    if (str.Document == null || str.Document.Length != 18)
                        str.Document = str.IspbString;

                    _source.Add(str);
                    bank = str;
                }

                if (bank.LongName.RemoveDiacritics().Equals(str.LongName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase) &&
                    bank.ShortName.RemoveDiacritics().Equals(str.ShortName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase))
                {
                    notFound++;
                    continue;
                }

                bank.LongName = str.LongName;
                bank.ShortName = str.ShortName;
                bank.DateUpdated = DateTimeOffset.Now;
                found++;
            }

            Console.WriteLine($"\r\nSTR | Found: {found} | Not found: {notFound}\r\n");

            return this;
        }

        /// <summary>
        /// Seeds the SLC.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Seeder.</returns>
        public Seeder SeedSlc(IEnumerable<Bank> items)
        {
            var found = 0;
            var notFound = 0;

            foreach (var slc in items)
            {
                var bank = _source.SingleOrDefault(b => b.Document != null && b.Document.Equals(slc.Document));

                if (bank == null)
                {
                    bank = _source.SingleOrDefault(b =>
                        b.LongName.RemoveDiacritics().Equals(slc.LongName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase) ||
                        (b.ShortName != null && b.ShortName.RemoveDiacritics().Equals(slc.LongName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase)));
                }

                if (bank == null)
                {
                    var ispb = int.Parse(slc.Document.RemoveNonNumeric()[..8]);

                    if (ispb == 0 && !slc.LongName.Equals("Banco do Brasil", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine($"SLC | ISPB zerado: {slc.LongName} | {slc.Document.Trim()}");
                        continue;
                    }

                    bank = _source.SingleOrDefault(b => b.Ispb.Equals(ispb) && b.LongName.Contains(slc.LongName, StringComparison.InvariantCultureIgnoreCase));
                }

                if (bank == null)
                {
                    Console.WriteLine($"SLC | Banco não encontrado: {slc.LongName} | {slc.Document.Trim()}");
                    notFound++;
                    continue;
                }

                if ((string.IsNullOrWhiteSpace(bank.Document) || bank.Document.Length != 18) &&
                    !string.IsNullOrWhiteSpace(slc.Document))
                {
                    bank.Document = slc.Document;
                    bank.DateUpdated = DateTimeOffset.Now;
                }

                else if (string.IsNullOrWhiteSpace(bank.Document) || bank.Document.Length != 18)
                    Console.WriteLine($"SLC | CNPJ inválido {slc.Compe} | {bank.Document} | {slc.Document}");

                if (string.IsNullOrWhiteSpace(bank.ShortName))
                {
                    bank.ShortName = slc.LongName;
                    bank.DateUpdated = DateTimeOffset.Now;
                }

                found++;

            }

            Console.WriteLine($"\r\nSLC | Found: {found} | Not found: {notFound}\r\n");

            return this;
        }

        /// <summary>
        /// Seeds the pix.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Seeder.</returns>
        public Seeder SeedPix(IEnumerable<Bank> items)
        {
            var found = 0;
            var upToDate = 0;
            var notFound = 0;


            foreach (var pix in items)
            {
                var bank = _source.SingleOrDefault(b =>
                    b.LongName.RemoveDiacritics().Equals(pix.LongName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase) ||
                    (b.ShortName != null && b.ShortName.RemoveDiacritics().Equals(pix.LongName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase)));

                if (bank == null)
                    bank = _source.SingleOrDefault(b => b.Ispb.Equals(pix.Ispb));

                if (bank == null)
                {
                    Console.WriteLine($"PIX | Participante não encontrado: {pix.LongName}");
                    notFound++;
                    continue;
                }

                if (bank.PixType != null &&
                    bank.PixType.Equals(pix.PixType) &&
                    bank.DatePixStarted != null &&
                    bank.DatePixStarted.Equals(pix.DatePixStarted))
                {
                    Console.WriteLine($"PIX | Participante já atualizado: {pix.LongName}");
                    upToDate++;
                    continue;
                }

                bank.PixType = pix.PixType;
                bank.DatePixStarted = pix.DatePixStarted;
                bank.DateUpdated = DateTimeOffset.Now;

                found++;
            }

            Console.WriteLine($"\r\nPIX | Found: {found} | Not found: {notFound} | Up to Date: {upToDate}\r\n");

            return this;
        }
    }
}