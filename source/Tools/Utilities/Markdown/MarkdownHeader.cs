﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using static Roslynator.Utilities.Markdown.MarkdownFactory;

namespace Roslynator.Utilities.Markdown
{
    public struct MarkdownHeader : IAppendable
    {
        internal MarkdownHeader(string text, HeaderLevel level = HeaderLevel.Header1)
        {
            OriginalText = text;
            Level = level;
        }

        public string OriginalText { get; }

        public HeaderLevel Level { get; }

        public StringBuilder Append(StringBuilder sb, MarkdownSettings settings = null)
        {
            return sb
                .Append(HeaderStart(Level))
                .Append(" ")
                .AppendLineIf(!string.IsNullOrEmpty(OriginalText), OriginalText, escape: true);
        }

        public override string ToString()
        {
            string s = HeaderStart(Level) + " ";

            if (!string.IsNullOrEmpty(OriginalText))
                s = s + OriginalText?.EscapeMarkdown() + Environment.NewLine;

            return s;
        }
    }
}
