﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Markdown
{
    //TODO: EmptyLineBetweenListItems
    public class MarkdownSettings
    {
        public MarkdownSettings(
            EmphasisStyle boldStyle = EmphasisStyle.Asterisk,
            EmphasisStyle italicStyle = EmphasisStyle.Asterisk,
            ListItemStyle listItemStyle = ListItemStyle.Asterisk,
            string horizontalRule = "___",
            HeadingOptions headingOptions = HeadingOptions.EmptyLineBeforeAndAfter,
            TableOptions tableOptions = TableOptions.FormatHeader | TableOptions.OuterPipe | TableOptions.Padding,
            EmptyLineOptions codeBlockOptions = EmptyLineOptions.EmptyLineBeforeAndAfter,
            bool allowLinkWithoutUrl = true,
            string indentChars = "  ",
            Func<char, bool> shouldBeEscaped = null)
        {
            BoldStyle = boldStyle;
            ItalicStyle = italicStyle;
            ListItemStyle = listItemStyle;
            HorizontalRule = horizontalRule;
            HeadingOptions = headingOptions;
            CodeBlockOptions = codeBlockOptions;
            TableOptions = tableOptions;
            AllowLinkWithoutUrl = allowLinkWithoutUrl;
            IndentChars = indentChars;
            ShouldBeEscaped = shouldBeEscaped ?? MarkdownEscaper.ShouldBeEscaped;
        }

        public static MarkdownSettings Default { get; } = new MarkdownSettings();

        public EmphasisStyle BoldStyle { get; }

        public EmphasisStyle AlternativeBoldStyle
        {
            get { return GetAlternativeEmphasisStyle(BoldStyle); }
        }

        public EmphasisStyle ItalicStyle { get; }

        public EmphasisStyle AlternativeItalicStyle
        {
            get { return GetAlternativeEmphasisStyle(ItalicStyle); }
        }

        public ListItemStyle ListItemStyle { get; }

        public string HorizontalRule { get; }

        public HeadingOptions HeadingOptions { get; }

        internal bool EmptyLineBeforeHeading
        {
            get { return (HeadingOptions & HeadingOptions.EmptyLineBefore) != 0; }
        }

        internal bool EmptyLineAfterHeading
        {
            get { return (HeadingOptions & HeadingOptions.EmptyLineAfter) != 0; }
        }

        public EmptyLineOptions CodeBlockOptions { get; }

        internal bool EmptyLineBeforeCodeBlock
        {
            get { return (CodeBlockOptions & EmptyLineOptions.EmptyLineBefore) != 0; }
        }

        internal bool EmptyLineAfterCodeBlock
        {
            get { return (CodeBlockOptions & EmptyLineOptions.EmptyLineAfter) != 0; }
        }

        public TableOptions TableOptions { get; }

        internal bool TablePadding
        {
            get { return (TableOptions & TableOptions.Padding) != 0; }
        }

        internal bool TableOuterPipe
        {
            get { return (TableOptions & TableOptions.OuterPipe) != 0; }
        }

        public bool AllowLinkWithoutUrl { get; }

        internal bool CloseHeading
        {
            get { return (HeadingOptions & HeadingOptions.Close) != 0; }
        }

        public string IndentChars { get; }

        public Func<char, bool> ShouldBeEscaped { get; }

        private static EmphasisStyle GetAlternativeEmphasisStyle(EmphasisStyle style)
        {
            if (style == EmphasisStyle.Asterisk)
                return EmphasisStyle.Underscore;

            if (style == EmphasisStyle.Underscore)
                return EmphasisStyle.Asterisk;

            throw new ArgumentException("", nameof(style));
        }
    }
}
