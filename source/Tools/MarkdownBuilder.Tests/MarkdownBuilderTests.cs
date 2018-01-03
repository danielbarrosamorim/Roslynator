﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Xunit;
using static Pihrtsoft.Markdown.MarkdownFactory;
using static Pihrtsoft.Markdown.Tests.TestHelpers;

#pragma warning disable CS1718

namespace Pihrtsoft.Markdown.Tests
{
    public class MarkdownBuilderTests
    {
        private const string Value = Chars;
        private const string ValueEscaped = CharsEscaped;

        [Theory]
        [InlineData("**", null)]
        [InlineData("**", EmphasisStyle.Asterisk)]
        [InlineData("__", EmphasisStyle.Underscore)]
        public void MarkdownBuilder_AppendBold(string syntax, EmphasisStyle? boldStyle)
        {
            const string x = Chars;
            const string y = CharsEscaped;
            MarkdownBuilder mb = CreateBuilderWithBoldStyle(boldStyle);

            Assert.Equal(syntax + y + syntax, mb.AppendBold(x).ToStringAndClear());
            Assert.Equal(syntax + y + syntax, mb.AppendBold((object)x).ToStringAndClear());
            Assert.Equal(syntax + y + y + syntax, mb.AppendBold(x, x).ToStringAndClear());
        }

        [Theory]
        [InlineData("**", null)]
        [InlineData("**", EmphasisStyle.Asterisk)]
        [InlineData("__", EmphasisStyle.Underscore)]
        public void MarkdownBuilder_Append_Bold(string syntax, EmphasisStyle? boldStyle)
        {
            const string x = Chars;
            const string y = CharsEscaped;

            MarkdownBuilder mb = CreateBuilderWithBoldStyle(boldStyle);
            Assert.Equal(syntax + y + syntax, mb.Append(Bold(x)).ToStringAndClear());
            Assert.Equal(syntax + y + syntax, mb.Append((object)Bold(x)).ToStringAndClear());
        }

        [Theory]
        [InlineData("*", null)]
        [InlineData("*", EmphasisStyle.Asterisk)]
        [InlineData("_", EmphasisStyle.Underscore)]
        public void MarkdownBuilder_AppendItalic(string syntax, EmphasisStyle? ItalicStyle)
        {
            const string x = Chars;
            const string y = CharsEscaped;
            MarkdownBuilder mb = CreateBuilderWithItalicStyle(ItalicStyle);

            Assert.Equal(syntax + y + syntax, mb.AppendItalic(x).ToStringAndClear());
            Assert.Equal(syntax + y + syntax, mb.AppendItalic((object)x).ToStringAndClear());
            Assert.Equal(syntax + y + y + syntax, mb.AppendItalic(x, x).ToStringAndClear());
        }

        [Theory]
        [InlineData("*", null)]
        [InlineData("*", EmphasisStyle.Asterisk)]
        [InlineData("_", EmphasisStyle.Underscore)]
        public void MarkdownBuilder_Append_Italic(string syntax, EmphasisStyle? italicStyle)
        {
            const string x = Chars;
            const string y = CharsEscaped;
            MarkdownBuilder mb = CreateBuilderWithItalicStyle(italicStyle);

            Assert.Equal(syntax + y + syntax, mb.Append(Italic(x)).ToStringAndClear());
            Assert.Equal(syntax + y + syntax, mb.Append((object)Italic(x)).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendStrikethrough()
        {
            const string x = Chars;
            const string y = CharsEscaped;
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("~~" + y + "~~", mb.AppendStrikethrough(x).ToStringAndClear());
            Assert.Equal("~~" + y + "~~", mb.AppendStrikethrough((object)x).ToStringAndClear());
            Assert.Equal("~~" + y + y + "~~", mb.AppendStrikethrough(x, x).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_Append_Strikethrough()
        {
            const string x = Chars;
            const string y = CharsEscaped;
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("~~" + y + "~~", mb.Append(Strikethrough(x)).ToStringAndClear());
            Assert.Equal("~~" + y + "~~", mb.Append((object)Strikethrough(x)).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendCode()
        {
            const string x = CharsEnclosedWithBacktick;
            const string y = CharsEnclosedWithBacktickDoubled;
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("` " + y + " `", mb.AppendCode(x).ToStringAndClear());
            Assert.Equal("` " + y + " `", mb.AppendCode((object)x).ToStringAndClear());
            Assert.Equal("` " + y + y + " `", mb.AppendCode(x, x).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendCode_String()
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("` `` `", mb.AppendCode("`").ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendCode_Char()
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("` `` `", mb.AppendCode('`').ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendCode_Params()
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("` ```` `", mb.AppendCode('`', '`').ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_Append_Code()
        {
            const string x = CharsEnclosedWithBacktick;
            const string y = CharsEnclosedWithBacktickDoubled;
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal("` " + y + " `", mb.Append(Code(x)).ToStringAndClear());
            Assert.Equal("` " + y + " `", mb.Append((object)Code(x)).ToStringAndClear());
        }

        [Theory]
        [InlineData("**", EmphasisOptions.Bold)]
        [InlineData("*", EmphasisOptions.Italic)]
        [InlineData("***", EmphasisOptions.BoldItalic)]
        [InlineData("~~", EmphasisOptions.Strikethrough)]
        public void MarkdownBuilder_Append_String_EmphasisOptions(string syntax, EmphasisOptions options)
        {
            const string x = Chars;
            const string y = CharsEscaped;
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal(syntax + y + syntax, mb.Append(x, options).ToStringAndClear());
            Assert.Equal(syntax + y + syntax, mb.Append(x, options).ToStringAndClear());
        }

        [Theory]
        [InlineData("` ", " `", EmphasisOptions.Code)]
        [InlineData("***~~` ", " `~~***", EmphasisOptions.Bold | EmphasisOptions.Italic | EmphasisOptions.Strikethrough | EmphasisOptions.Code)]
        public void MarkdownBuilder_Append_String_EmphasisOptionsCode(string open, string close, EmphasisOptions options)
        {
            const string x = CharsEnclosedWithBacktick;
            const string y = CharsEnclosedWithBacktickDoubled;
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal(open + y + close, mb.Append(x, options).ToStringAndClear());
            Assert.Equal(open + y + close, mb.Append(x, options).ToStringAndClear());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(HeadingOptions.None)]
        [InlineData(HeadingOptions.UnderlineH2)]
        public void MarkdownBuilder_AppendHeading1(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal($"# {ValueEscaped}{NewLine}", mb.AppendHeading1(Value).ToStringAndClear());
            Assert.Equal("# " + NewLine, mb.AppendHeading1().ToStringAndClear());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(HeadingOptions.None)]
        [InlineData(HeadingOptions.UnderlineH1)]
        public void MarkdownBuilder_AppendHeading2(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal($"## {ValueEscaped}{NewLine}", mb.AppendHeading2(Value).ToStringAndClear());
            Assert.Equal("## " + NewLine, mb.AppendHeading2().ToStringAndClear());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(HeadingOptions.None)]
        [InlineData(HeadingOptions.UnderlineH1)]
        [InlineData(HeadingOptions.UnderlineH2)]
        [InlineData(HeadingOptions.Underline)]
        public void MarkdownBuilder_AppendHeading3(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal($"### {ValueEscaped}{NewLine}", mb.AppendHeading3(Value).ToStringAndClear());
            Assert.Equal("### " + NewLine, mb.AppendHeading3().ToStringAndClear());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(HeadingOptions.None)]
        [InlineData(HeadingOptions.UnderlineH1)]
        [InlineData(HeadingOptions.UnderlineH2)]
        [InlineData(HeadingOptions.Underline)]
        public void MarkdownBuilder_AppendHeading4(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal($"#### {ValueEscaped}{NewLine}", mb.AppendHeading4(Value).ToStringAndClear());
            Assert.Equal("#### " + NewLine, mb.AppendHeading4().ToStringAndClear());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(HeadingOptions.None)]
        [InlineData(HeadingOptions.UnderlineH1)]
        [InlineData(HeadingOptions.UnderlineH2)]
        [InlineData(HeadingOptions.Underline)]
        public void MarkdownBuilder_AppendHeading5(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal($"##### {ValueEscaped}{NewLine}", mb.AppendHeading5(Value).ToStringAndClear());
            Assert.Equal("##### " + NewLine, mb.AppendHeading5().ToStringAndClear());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(HeadingOptions.None)]
        [InlineData(HeadingOptions.UnderlineH1)]
        [InlineData(HeadingOptions.UnderlineH2)]
        [InlineData(HeadingOptions.Underline)]
        public void MarkdownBuilder_AppendHeading6(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal($"###### {ValueEscaped}{NewLine}", mb.AppendHeading6(Value).ToStringAndClear());
            Assert.Equal("###### " + NewLine, mb.AppendHeading6().ToStringAndClear());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void MarkdownBuilder_AppendHeading(int level)
        {
            MarkdownBuilder mb = CreateBuilder(new MarkdownSettings(headingOptions: HeadingOptions.None));

            Assert.Equal($"{new string('#', level)} {ValueEscaped}{NewLine}", mb.AppendHeading(level, Value).ToStringAndClear());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(7)]
        public void MarkdownBuilder_AppendHeading_Throws(int level)
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Throws<ArgumentOutOfRangeException>(() => mb.AppendHeading(level, Value));
        }

        [Fact]
        public void MarkdownBuilder_AppendHeading_Params()
        {
            MarkdownBuilder mb = CreateBuilder(new MarkdownSettings(headingOptions: HeadingOptions.None));
        }

        [Theory]
        [InlineData(HeadingOptions.UnderlineH1)]
        [InlineData(HeadingOptions.Underline)]
        public void MarkdownBuilder_AppendHeading_UnderlineH1(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal(ValueEscaped + NewLine + new string('=', ValueEscaped.Length) + NewLine, mb.AppendHeading1(Value).ToStringAndClear());
        }

        [Theory]
        [InlineData(HeadingOptions.UnderlineH2)]
        [InlineData(HeadingOptions.Underline)]
        public void MarkdownBuilder_AppendHeading_UnderlineH2(HeadingOptions? options)
        {
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(options);

            Assert.Equal(ValueEscaped + NewLine + new string('-', ValueEscaped.Length) + NewLine, mb.AppendHeading2(Value).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendHeading_EmptyLineBefore()
        {
            string text = HeadingText();
            const string s = "# " + CharsEscaped + NewLine;
            Heading heading = Heading(Chars, 1);
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(HeadingOptions.EmptyLineBefore);

            Assert.Equal(
                text + NewLine2 + s + NewLine + s + text,
                mb.Append(text).Append(heading).Append(heading).Append(text).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendHeading_EmptyLineAfter()
        {
            string text = HeadingText();
            const string s = "# " + CharsEscaped + NewLine;
            Heading heading = Heading(Chars, 1);
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(HeadingOptions.EmptyLineAfter);

            Assert.Equal(
                text + NewLine + s + NewLine + s + NewLine + text,
                mb.Append(text).Append(heading).Append(heading).Append(text).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendHeading_EmptyLineBeforeAfter()
        {
            string text = HeadingText();
            const string s = "# " + CharsEscaped + NewLine;
            Heading heading = Heading(Chars, 1);
            MarkdownBuilder mb = CreateBuilderWithHeadingOptions(HeadingOptions.EmptyLineBeforeAndAfter);

            Assert.Equal(
                text + NewLine2 + s + NewLine + s + NewLine + text,
                mb.Append(text).Append(heading).Append(heading).Append(text).ToStringAndClear());
        }

        [Theory]
        [InlineData('*', HorizontalRuleStyle.Asterisk)]
        [InlineData('-', HorizontalRuleStyle.Hyphen)]
        [InlineData('_', HorizontalRuleStyle.Underscore)]
        public void MarkdownBuilder_AppendHorizontalRule(char syntax, HorizontalRuleStyle style)
        {
            for (int i = 3; i <= 5; i++)
            {
                MarkdownBuilder mb = CreateBuilder();

                Assert.Equal(new string(syntax, i) + NewLine, mb.Append(HorizontalRule(style: style, count: i, addSpaces: false)).ToStringAndClear());
            }

            for (int i = 3; i <= 5; i++)
            {
                MarkdownBuilder mb = CreateBuilder();

                Assert.Equal(string.Join(" ", Enumerable.Repeat<char>(syntax, i)) + NewLine, mb.Append(HorizontalRule(style: style, count: i, addSpaces: true)).ToStringAndClear());
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(2)]
        public void MarkdownBuilder_AppendHorizontalRule_Throws(int count)
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Throws<ArgumentOutOfRangeException>(() => mb.AppendHorizontalRule(style: HorizontalRuleStyle.Asterisk, count: count, addSpaces: false));
            Assert.Throws<ArgumentOutOfRangeException>(() => mb.AppendHorizontalRule(style: HorizontalRuleStyle.Hyphen, count: count, addSpaces: false));
            Assert.Throws<ArgumentOutOfRangeException>(() => mb.AppendHorizontalRule(style: HorizontalRuleStyle.Underscore, count: count, addSpaces: false));
        }

        [Fact]
        public void MarkdownBuilder_AppendImage()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "ImageText";
            const string url = "ImageUrl";
            const string title = "ImageTitle";

            Image image = Image(text + Chars, url + CharsWithoutSpaces, title + Chars);

            string expected = $"![{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped} \"{title + CharsDoubleQuoteEscaped}\")";

            Assert.Equal(expected, mb.AppendImage(image.Text, image.Url, image.Title).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendImage_NoTitle()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "ImageText";
            const string url = "ImageUrl";

            Image image = Image(text + Chars, url + CharsWithoutSpaces);

            string expected = $"![{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped})";

            Assert.Equal(expected, mb.AppendImage(image.Text, image.Url).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_Append_Image()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "ImageText";
            const string url = "ImageUrl";
            const string title = "ImageTitle";

            Image image = Image(text + Chars, url + CharsWithoutSpaces, title + Chars);

            string expected = $"![{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped} \"{title + CharsDoubleQuoteEscaped}\")";

            Assert.Equal(expected, mb.Append(image).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)image).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_Append_Image_NoTitle()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "ImageText";
            const string url = "ImageUrl";

            Image image = Image(text + Chars, url + CharsWithoutSpaces);

            string expected = $"![{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped})";

            Assert.Equal(expected, mb.Append(image).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)image).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendImage_Throws()
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => mb.AppendImage(text: null, url: "Url"));
            Assert.Throws<ArgumentNullException>(() => mb.AppendImage(text: "Text", url: null));
        }

        [Fact]
        public void MarkdownBuilder_AppendLink()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "LinkText";
            const string url = "LinkUrl";
            const string title = "LinkTitle";

            Link image = Link(text + Chars, url + CharsWithoutSpaces, title + Chars);

            string expected = $"[{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped} \"{title + CharsDoubleQuoteEscaped}\")";

            Assert.Equal(expected, mb.AppendLink(image.Text, image.Url, image.Title).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendLink_NoTitle()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "LinkText";
            const string url = "LinkUrl";

            Link image = Link(text + Chars, url + CharsWithoutSpaces);

            string expected = $"[{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped})";

            Assert.Equal(expected, mb.AppendLink(image.Text, image.Url).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_Append_Link()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "LinkText";
            const string url = "LinkUrl";
            const string title = "LinkTitle";

            Link image = Link(text + Chars, url + CharsWithoutSpaces, title + Chars);

            string expected = $"[{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped} \"{title + CharsDoubleQuoteEscaped}\")";

            Assert.Equal(expected, mb.Append(image).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)image).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_Append_Link_NoTitle()
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "LinkText";
            const string url = "LinkUrl";

            Link image = Link(text + Chars, url + CharsWithoutSpaces);

            string expected = $"[{text + CharsSquareBracketsBacktickEscaped}]({url + CharsWithoutSpacesParenthesesEscaped})";

            Assert.Equal(expected, mb.Append(image).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)image).ToStringAndClear());
        }

        [Theory]
        [InlineData("*", null)]
        [InlineData("*", ListItemStyle.Asterisk)]
        [InlineData("-", ListItemStyle.Minus)]
        [InlineData("+", ListItemStyle.Plus)]
        public void MarkdownBuilder_AppendListItem(string syntax, ListItemStyle? style)
        {
            MarkdownBuilder mb = CreateBuilderWithListItemStyle(style);
            const string text = "ListItemText";
            string expected = syntax + $" {text + CharsEscaped}" + NewLine;
            ListItem item = ListItem(text + Chars);

            Assert.Equal(expected, mb.AppendListItem(item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendListItem((object)item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendListItem(item.Text, null).ToStringAndClear());
        }

        [Theory]
        [InlineData("*", null)]
        [InlineData("*", ListItemStyle.Asterisk)]
        [InlineData("-", ListItemStyle.Minus)]
        [InlineData("+", ListItemStyle.Plus)]
        public void MarkdownBuilder_Append_ListItem(string syntax, ListItemStyle? style)
        {
            MarkdownBuilder mb = CreateBuilderWithListItemStyle(style);
            const string text = "ListItemText";
            string expected = syntax + $" {text + CharsEscaped}" + NewLine;
            ListItem item = ListItem(text + Chars);

            Assert.Equal(expected, mb.Append(item).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)item).ToStringAndClear());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void MarkdownBuilder_AppendOrderedListItem(int number)
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "OrderedListItemText";

            string expected = number + $". {text + CharsEscaped}" + NewLine;
            OrderedListItem item = OrderedListItem(number, text + Chars);

            Assert.Equal(expected, mb.AppendOrderedListItem(number, item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendOrderedListItem(number, (object)item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendOrderedListItem(number, item.Text, null).ToStringAndClear());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void MarkdownBuilder_Append_OrderedListItem(int number)
        {
            MarkdownBuilder mb = CreateBuilder();

            const string text = "OrderedListItemText";

            string expected = number + $". {text + CharsEscaped}" + NewLine;
            OrderedListItem item = OrderedListItem(number, text + Chars);

            Assert.Equal(expected, mb.Append(item).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)item).ToStringAndClear());
        }

        [Theory]
        [InlineData(-3)]
        [InlineData(-2)]
        [InlineData(-1)]
        public void MarkdownBuilder_AppendOrderedListItem_Throws(int number)
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Throws<ArgumentOutOfRangeException>(() => mb.AppendOrderedListItem(number));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(Chars, CharsEscaped)]
        public void MarkdownBuilder_AppendTaskListItem(string text, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            const string start = "- [ ] ";
            string expected = start + text2 + CharsEscaped + NewLine;
            TaskListItem item = TaskListItem(text + Chars);

            Assert.Equal(expected, mb.AppendTaskListItem(item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendTaskListItem((object)item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendTaskListItem(item.Text, null).ToStringAndClear());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(Chars, CharsEscaped)]
        public void MarkdownBuilder_Append_TaskListItem(string text, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            const string start = "- [ ] ";
            string expected = start + text2 + CharsEscaped + NewLine;
            TaskListItem item = TaskListItem(text + Chars);

            Assert.Equal(expected, mb.Append(item).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)item).ToStringAndClear());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(Chars, CharsEscaped)]
        public void MarkdownBuilder_Append_TaskListItem_NotCompleted(string text, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            const string start = "- [ ] ";
            string expected = start + text2 + CharsEscaped + NewLine;
            TaskListItem item = TaskListItem(text + Chars, isCompleted: false);

            Assert.Equal(expected, mb.Append(item).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)item).ToStringAndClear());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(Chars, CharsEscaped)]
        public void MarkdownBuilder_Append_TaskListItem_Completed(string text, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            const string start = "- [x] ";
            string expected = start + text2 + CharsEscaped + NewLine;
            TaskListItem item = TaskListItem(text + Chars, isCompleted: true);

            Assert.Equal(expected, mb.Append(item).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)item).ToStringAndClear());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(Chars, CharsEscaped)]
        public void MarkdownBuilder_AppendCompletedTaskListItem(string text, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            const string start = "- [x] ";
            string expected = start + text2 + CharsEscaped + NewLine;
            TaskListItem item = CompletedTaskListItem(text + Chars);

            Assert.Equal(expected, mb.AppendCompletedTaskListItem(item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendCompletedTaskListItem((object)item.Text).ToStringAndClear());
            Assert.Equal(expected, mb.AppendCompletedTaskListItem(item.Text, null).ToStringAndClear());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(Chars, CharsEscaped)]
        public void MarkdownBuilder_Append_CompletedTaskListItem(string text, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            const string start = "- [x] ";
            string expected = start + text2 + CharsEscaped + NewLine;
            TaskListItem item = CompletedTaskListItem(text + Chars);

            Assert.Equal(expected, mb.Append(item).ToStringAndClear());
            Assert.Equal(expected, mb.Append((object)item).ToStringAndClear());
        }

        [Theory]
        [InlineData(Chars, "> " + CharsEscaped + NewLine)]
        [InlineData(Chars + NewLine + Chars, "> " + CharsEscaped + NewLine + "> " + CharsEscaped + NewLine)]
        public void MarkdownBuilder_AppendQuoteBlock(string text1, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Equal(text2, mb.AppendQuoteBlock(text1).ToStringAndClear());
        }

        [Theory]
        [InlineData(Chars, "> " + CharsEscaped + NewLine)]
        [InlineData(Chars + NewLine + Chars, "> " + CharsEscaped + NewLine + "> " + CharsEscaped + NewLine)]
        public void MarkdownBuilder_Append_QuoteBlock(string text1, string text2)
        {
            MarkdownBuilder mb = CreateBuilder();
            QuoteBlock quoteBlock = QuoteBlock(text1);

            Assert.Equal(text2, mb.Append(quoteBlock).ToStringAndClear());
            Assert.Equal(text2, mb.Append((object)quoteBlock).ToStringAndClear());
        }

        [Fact]
        public void MarkdownBuilder_AppendQuoteBlock_Throws()
        {
            MarkdownBuilder mb = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => mb.AppendQuoteBlock(null));
        }
    }
}
