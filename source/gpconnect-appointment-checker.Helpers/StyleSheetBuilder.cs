using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace gpconnect_appointment_checker.Helpers
{
    public static class StyleSheetBuilder
    {
        public static Stylesheet CreateStylesheet()
        {
            Stylesheet ss = new Stylesheet();

            Fonts fonts = GetFonts();
            ss.Append(fonts);

            Fills fills = GetFills();
            ss.Append(fills);

            Borders borders = GetBorders();
            ss.Append(borders);

            CellStyleFormats cellStyleFormats = GetCellStyleFormats();
            ss.Append(cellStyleFormats);

            CellFormats cellFormats = GetCellFormats();
            ss.Append(cellFormats);

            CellStyles cellStyles = GetCellStyles();
            ss.Append(cellStyles);

            return ss;
        }

        private static Fonts GetFonts()
        {
            Fonts fonts = new Fonts();

            // 0
            Font fontNormal = new Font()
            {
                FontName = new FontName { Val = StringValue.FromString("Calibri") },
                FontSize = new FontSize { Val = DoubleValue.FromDouble(11) }
            };
            fonts.Append(fontNormal);

            // 1
            Font fontTitle = new Font()
            {
                FontName = new FontName { Val = StringValue.FromString("Calibri") },
                FontSize = new FontSize { Val = DoubleValue.FromDouble(25) },
                Color = new Color { Rgb = HexBinaryValue.FromString("ff584642") }
            };
            fonts.Append(fontTitle);

            // 2
            Font fontColumnHeaders = new Font()
            {
                FontName = new FontName { Val = StringValue.FromString("Calibri") },
                FontSize = new FontSize { Val = DoubleValue.FromDouble(13.5) },
                Color = new Color() { Rgb = HexBinaryValue.FromString("ffffffff") }, //white
                Bold = new Bold() { Val = true }
            };
            fonts.Append(fontColumnHeaders);

            fonts.Count = UInt32Value.FromUInt32((uint)fonts.ChildElements.Count);
            return fonts;
        }

        private static Fills GetFills()
        {
            Fills fills = new Fills();

            var defaultFill1 = new Fill(new PatternFill { PatternType = PatternValues.None }); // Index 0 - default
            fills.Append(defaultFill1);

            var defaultFill2 = new Fill(new PatternFill { PatternType = PatternValues.Gray125 }); // Index 1 - default
            fills.Append(defaultFill2);

            Fill fillColumnHeader = new Fill
            {
                PatternFill = new PatternFill
                {
                    PatternType = PatternValues.Solid,
                    ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FF005EB8") }
                }
            };
            fills.Append(fillColumnHeader);

            fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);
            return fills;
        }

        private static Borders GetBorders()
        {
            Borders borders = new Borders();

            // 1
            Border borderTitle = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };

            borders.Append(borderTitle);

            borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);

            return borders;
        }

        private static CellStyleFormats GetCellStyleFormats()
        {
            CellStyleFormats cellStyleFormats = new CellStyleFormats();

            CellFormat defaultFormat = new CellFormat
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 0,
                FillId = 0,
                NumberFormatId = 0,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = true,
                ApplyNumberFormat = true
            };
            cellStyleFormats.Append(defaultFormat);

            CellFormat cellFormatTitle = new CellFormat
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 1,
                FillId = 0,
                NumberFormatId = 0,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = true,
                ApplyNumberFormat = true
            };
            cellStyleFormats.Append(cellFormatTitle);

            CellFormat cellFormatColumnHeader = new CellFormat
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 2,
                FillId = 2,
                NumberFormatId = 0,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = false,
                ApplyNumberFormat = true
            };
            cellStyleFormats.Append(cellFormatColumnHeader);

            CellFormat cellFormatNormal = new CellFormat
            {
                ApplyAlignment = true,
                BorderId = 0, // borderTitle
                FontId = 0, // fontTitle
                FillId = 0, // fillTitle,
                NumberFormatId = 14,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = true,
                ApplyNumberFormat = true
            };
            cellStyleFormats.Append(cellFormatNormal);

            cellStyleFormats.Count = UInt32Value.FromUInt32((uint)cellStyleFormats.ChildElements.Count);

            return cellStyleFormats;
        }

        private static CellFormats GetCellFormats()
        {
            CellFormats cellFormats = new CellFormats();

            // index 0
            CellFormat cellFormatTitle = new CellFormat
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 0,
                FillId = 0,
                NumberFormatId = 0,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = true,
                ApplyNumberFormat = true,
                FormatId = 0,
                Alignment = new Alignment
                {
                    ReadingOrder = 0,
                    ShrinkToFit = false,
                    JustifyLastLine = false,
                    RelativeIndent = 0,
                    Indent = 0,
                    WrapText = false,
                    TextRotation = 0,
                    Vertical = VerticalAlignmentValues.Bottom,
                    Horizontal = HorizontalAlignmentValues.General
                }
            };
            cellFormats.Append(cellFormatTitle);

            // index 1
            CellFormat cellFormatColumnHeader = new CellFormat
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 1,
                FillId = 0,
                NumberFormatId = 0,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = true,
                ApplyNumberFormat = true,
                FormatId = 0,
                Alignment = new Alignment
                {
                    ReadingOrder = 0,
                    ShrinkToFit = false,
                    JustifyLastLine = false,
                    RelativeIndent = 0,
                    Indent = 0,
                    WrapText = false,
                    TextRotation = 0,
                    Vertical = VerticalAlignmentValues.Center,
                    Horizontal = HorizontalAlignmentValues.General
                }
            };
            cellFormats.Append(cellFormatColumnHeader);

            // index 2
            CellFormat cellFormatNormal = new CellFormat()
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 2,
                FillId = 2,
                NumberFormatId = 0,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = false,
                ApplyNumberFormat = true,
                FormatId = 0,
                Alignment = new Alignment
                {
                    ReadingOrder = 0,
                    ShrinkToFit = false,
                    JustifyLastLine = false,
                    RelativeIndent = 0,
                    Indent = 0,
                    WrapText = false,
                    TextRotation = 0,
                    Vertical = VerticalAlignmentValues.Top,
                    Horizontal = HorizontalAlignmentValues.General
                }
            };
            cellFormats.Append(cellFormatNormal);

            CellFormat cellFormat4 = new CellFormat()
            {
                ApplyAlignment = true,
                BorderId = 0,
                FontId = 0,
                FillId = 0,
                NumberFormatId = 14,
                ApplyProtection = true,
                ApplyBorder = false,
                ApplyFill = true,
                ApplyNumberFormat = true,
                FormatId = 0,
                Alignment = new Alignment
                {
                    ReadingOrder = 0,
                    ShrinkToFit = false,
                    JustifyLastLine = false,
                    RelativeIndent = 0,
                    Indent = 0,
                    WrapText = false,
                    TextRotation = 0,
                    Vertical = VerticalAlignmentValues.Bottom,
                    Horizontal = HorizontalAlignmentValues.General
                }
            };
            cellFormats.Append(cellFormat4);

            cellFormats.Count = UInt32Value.FromUInt32((uint)cellFormats.ChildElements.Count);

            return cellFormats;
        }

        private static CellStyles GetCellStyles()
        {
            CellStyles cellStyles = new CellStyles();

            CellStyle normalStyle = new CellStyle
            {
                Name = StringValue.FromString("Normal"),
                FormatId = 0,
                BuiltinId = 0
            };
            cellStyles.Append(normalStyle);

            cellStyles.Count = UInt32Value.FromUInt32((uint)cellStyles.ChildElements.Count);
            return cellStyles;
        }

    }
}