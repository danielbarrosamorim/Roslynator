﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Pihrtsoft.Markdown
{
    [DebuggerDisplay("{Text,nq}")]
    public struct QuoteBlock : IMarkdown
    {
        internal QuoteBlock(string text)
        {
            Text = text;
        }

        public string Text { get; }

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            return mb.AppendQuoteBlock(Text);
        }
    }
}
