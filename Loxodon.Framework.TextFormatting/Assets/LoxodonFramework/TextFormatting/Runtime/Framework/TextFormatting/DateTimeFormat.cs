﻿// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==

//
// This class is a GC-free version of DateTimeFormat, which is modified based on Microsoft's DateTimeFormat.
// https://github.com/microsoft/referencesource/blob/master/mscorlib/system/globalization/datetimeformat.cs
// Author Clark
// 

namespace Loxodon.Framework.TextFormatting {
    using System;
    using System.Text;
    using System.Threading;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Collections.Concurrent;

    /*
     Customized format patterns:
     P.S. Format in the table below is the internal number format used to display the pattern.

     Patterns   Format      Description                           Example
     =========  ==========  ===================================== ========
        "h"     "0"         hour (12-hour clock)w/o leading zero  3
        "hh"    "00"        hour (12-hour clock)with leading zero 03
        "hh*"   "00"        hour (12-hour clock)with leading zero 03

        "H"     "0"         hour (24-hour clock)w/o leading zero  8
        "HH"    "00"        hour (24-hour clock)with leading zero 08
        "HH*"   "00"        hour (24-hour clock)                  08

        "m"     "0"         minute w/o leading zero
        "mm"    "00"        minute with leading zero
        "mm*"   "00"        minute with leading zero

        "s"     "0"         second w/o leading zero
        "ss"    "00"        second with leading zero
        "ss*"   "00"        second with leading zero

        "f"     "0"         second fraction (1 digit)
        "ff"    "00"        second fraction (2 digit)
        "fff"   "000"       second fraction (3 digit)
        "ffff"  "0000"      second fraction (4 digit)
        "fffff" "00000"         second fraction (5 digit)
        "ffffff"    "000000"    second fraction (6 digit)
        "fffffff"   "0000000"   second fraction (7 digit)

        "F"     "0"         second fraction (up to 1 digit)
        "FF"    "00"        second fraction (up to 2 digit)
        "FFF"   "000"       second fraction (up to 3 digit)
        "FFFF"  "0000"      second fraction (up to 4 digit)
        "FFFFF" "00000"         second fraction (up to 5 digit)
        "FFFFFF"    "000000"    second fraction (up to 6 digit)
        "FFFFFFF"   "0000000"   second fraction (up to 7 digit)

        "t"                 first character of AM/PM designator   A
        "tt"                AM/PM designator                      AM
        "tt*"               AM/PM designator                      PM

        "d"     "0"         day w/o leading zero                  1
        "dd"    "00"        day with leading zero                 01
        "ddd"               short weekday name (abbreviation)     Mon
        "dddd"              full weekday name                     Monday
        "dddd*"             full weekday name                     Monday


        "M"     "0"         month w/o leading zero                2
        "MM"    "00"        month with leading zero               02
        "MMM"               short month name (abbreviation)       Feb
        "MMMM"              full month name                       Febuary
        "MMMM*"             full month name                       Febuary

        "y"     "0"         two digit year (year % 100) w/o leading zero           0
        "yy"    "00"        two digit year (year % 100) with leading zero          00
        "yyy"   "D3"        year                                  2000
        "yyyy"  "D4"        year                                  2000
        "yyyyy" "D5"        year                                  2000
        ...

        "z"     "+0;-0"     timezone offset w/o leading zero      -8
        "zz"    "+00;-00"   timezone offset with leading zero     -08
        "zzz"      "+00;-00" for hour offset, "00" for minute offset  full timezone offset   -07:30
        "zzz*"  "+00;-00" for hour offset, "00" for minute offset   full timezone offset   -08:00

        "K"    -Local       "zzz", e.g. -08:00
               -Utc         "'Z'", representing UTC
               -Unspecified ""
               -DateTimeOffset      "zzzzz" e.g -07:30:15

        "g*"                the current era name                  A.D.

        ":"                 time separator                        : -- DEPRECATED - Insert separator directly into pattern (eg: "H.mm.ss")
        "/"                 date separator                        /-- DEPRECATED - Insert separator directly into pattern (eg: "M-dd-yyyy")
        "'"                 quoted string                         'ABC' will insert ABC into the formatted string.
        '"'                 quoted string                         "ABC" will insert ABC into the formatted string.
        "%"                 used to quote a single pattern characters      E.g.The format character "%y" is to print two digit year.
        "\"                 escaped character                     E.g. '\d' insert the character 'd' into the format string.
        other characters    insert the character into the format string.

    Pre-defined format characters:
        (U) to indicate Universal time is used.
        (G) to indicate Gregorian calendar is used.

        Format              Description                             Real format                             Example
        =========           =================================       ======================                  =======================
        "d"                 short date                              culture-specific                        10/31/1999
        "D"                 long data                               culture-specific                        Sunday, October 31, 1999
        "f"                 full date (long date + short time)      culture-specific                        Sunday, October 31, 1999 2:00 AM
        "F"                 full date (long date + long time)       culture-specific                        Sunday, October 31, 1999 2:00:00 AM
        "g"                 general date (short date + short time)  culture-specific                        10/31/1999 2:00 AM
        "G"                 general date (short date + long time)   culture-specific                        10/31/1999 2:00:00 AM
        "m"/"M"             Month/Day date                          culture-specific                        October 31
(G)     "o"/"O"             Round Trip XML                          "yyyy-MM-ddTHH:mm:ss.fffffffK"          1999-10-31 02:00:00.0000000Z
(G)     "r"/"R"             RFC 1123 date,                          "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"   Sun, 31 Oct 1999 10:00:00 GMT
(G)     "s"                 Sortable format, based on ISO 8601.     "yyyy-MM-dd'T'HH:mm:ss"                 1999-10-31T02:00:00
                                                                    ('T' for local time)
        "t"                 short time                              culture-specific                        2:00 AM
        "T"                 long time                               culture-specific                        2:00:00 AM
(G)     "u"                 Universal time with sortable format,    "yyyy'-'MM'-'dd HH':'mm':'ss'Z'"        1999-10-31 10:00:00Z
                            based on ISO 8601.
(U)     "U"                 Universal time with full                culture-specific                        Sunday, October 31, 1999 10:00:00 AM
                            (long date + long time) format
                            "y"/"Y"             Year/Month day                          culture-specific                        October, 1999

    */

    //This class contains only static members and does not require the serializable attribute.
    internal static class DateTimeFormat {

        internal const int MaxSecondsFractionDigits = 7;
        internal static readonly TimeSpan NullOffset = TimeSpan.MinValue;

        internal static char[] allStandardFormats = {
            'd', 'D', 'f', 'F', 'g', 'G',
            'm', 'M', 'o', 'O', 'r', 'R',
            's', 't', 'T', 'u', 'U', 'y', 'Y',
        };

        internal const string RoundtripFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";
        internal const string RoundtripDateTimeUnfixed = "yyyy'-'MM'-'ddTHH':'mm':'ss zzz";

        private const int DEFAULT_ALL_DATETIMES_SIZE = 132;

        internal static readonly DateTimeFormatInfo InvariantFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
        internal static readonly string[] InvariantAbbreviatedMonthNames = InvariantFormatInfo.AbbreviatedMonthNames;
        internal static readonly string[] InvariantAbbreviatedDayNames = InvariantFormatInfo.AbbreviatedDayNames;
        internal const string Gmt = "GMT";

        internal static string[] fixedNumberFormats = new string[] {
            "0",
            "00",
            "000",
            "0000",
            "00000",
            "000000",
            "0000000",
        };
        internal static GregorianCalendar DEFAULT_GREGORIAN_CALENDAR = new GregorianCalendar();
        internal const string JapaneseEraStart = "\u5143";//datetimeformatinfo.cs
        internal const string CJKYearSuff = "\u5e74"; //datetimeformatinfoscanner.cs
        internal static readonly string SwitchFormatJapaneseFirstYearAsANumber = "Switch.System.Globalization.FormatJapaneseFirstYearAsANumber";
        internal static int _formatJapaneseFirstYearAsANumber = 0;
        ////////////////////////////////////////////////////////////////////////////
        //
        // Format the positive integer value to a string and perfix with assigned
        // length of leading zero.
        //
        // Parameters:
        //  value: The value to format
        //  len: The maximum length for leading zero.
        //  If the digits of the value is greater than len, no leading zero is added.
        //
        // Notes:
        //  The function can format to Int32.MaxValue.
        //
        ////////////////////////////////////////////////////////////////////////////
        internal static void FormatDigits(int value, int len, ref ValueStringBuilder result) {
            Contract.Assert(value >= 0, "DateTimeFormat.FormatDigits(): value >= 0");
            FormatDigits(value, len, false, ref result);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        internal unsafe static void FormatDigits(int value, int len, bool overrideLengthLimit, ref ValueStringBuilder result) {
            Contract.Assert(value >= 0, "DateTimeFormat.FormatDigits(): value >= 0");

            // Limit the use of this function to be two-digits, so that we have the same behavior
            // as RTM bits.
            if (!overrideLengthLimit && len > 2) {
                len = 2;
            }

            char* buffer = stackalloc char[16];
            char* p = buffer + 16;
            int n = value;
            do {
                *--p = (char)(n % 10 + '0');
                n /= 10;
            } while ((n != 0) && (p > buffer));

            int digits = (int)(buffer + 16 - p);

            //If the repeat count is greater than 0, we're trying
            //to emulate the "00" format, so we have to prepend
            //a zero if the string only has one character.
            while ((digits < len) && (p > buffer)) {
                *--p = '0';
                digits++;
            }
            result.Append(p, digits);
        }

        private static void HebrewFormatDigits(int digits, ref ValueStringBuilder result) {
            HebrewNumber.ToString(digits, ref result);
        }

        internal static int ParseRepeatPattern(ReadOnlySpan<char> format, int pos, char patternChar) {
            int len = format.Length;
            int index = pos + 1;
            while ((index < len) && (format[index] == patternChar)) {
                index++;
            }
            return (index - pos);
        }

        private static string FormatDayOfWeek(int dayOfWeek, int repeat, DateTimeFormatInfo dtfi) {
            Contract.Assert(dayOfWeek >= 0 && dayOfWeek <= 6, "dayOfWeek >= 0 && dayOfWeek <= 6");
            if (repeat == 3) {
                return dtfi.GetAbbreviatedDayName((DayOfWeek)dayOfWeek);
            }
            // Call dtfi.GetDayName() here, instead of accessing DayNames property, because we don't
            // want a clone of DayNames, which will hurt perf.
            return dtfi.GetDayName((DayOfWeek)dayOfWeek);
        }

        private static string FormatMonth(int month, int repeatCount, DateTimeFormatInfo dtfi) {
            Contract.Assert(month >= 1 && month <= 12, "month >=1 && month <= 12");
            if (repeatCount == 3) {
                return dtfi.GetAbbreviatedMonthName(month);
            }
            // Call GetMonthName() here, instead of accessing MonthNames property, because we don't
            // want a clone of MonthNames, which will hurt perf.
            return dtfi.GetMonthName(month);
        }

        //
        //  FormatHebrewMonthName
        //
        //  Action: Return the Hebrew month name for the specified DateTime.
        //  Returns: The month name string for the specified DateTime.
        //  Arguments:
        //        time   the time to format
        //        month  The month is the value of HebrewCalendar.GetMonth(time).
        //        repeat Return abbreviated month name if repeat=3, or full month name if repeat=4
        //        dtfi    The DateTimeFormatInfo which uses the Hebrew calendars as its calendar.
        //  Exceptions: None.
        //

        /* Note:
            If DTFI is using Hebrew calendar, GetMonthName()/GetAbbreviatedMonthName() will return month names like this:
            1   Hebrew 1st Month
            2   Hebrew 2nd Month
            ..  ...
            6   Hebrew 6th Month
            7   Hebrew 6th Month II (used only in a leap year)
            8   Hebrew 7th Month
            9   Hebrew 8th Month
            10  Hebrew 9th Month
            11  Hebrew 10th Month
            12  Hebrew 11th Month
            13  Hebrew 12th Month

            Therefore, if we are in a regular year, we have to increment the month name if moth is greater or eqaul to 7.
        */
        private static string FormatHebrewMonthName(DateTime time, int month, int repeatCount, DateTimeFormatInfo dtfi) {
            Contract.Assert(repeatCount != 3 || repeatCount != 4, "repeateCount should be 3 or 4");
            if (dtfi.Calendar.IsLeapYear(dtfi.Calendar.GetYear(time))) {
                // This month is in a leap year
                return GetMonthName(dtfi, month, 2, true);// dtfi.internalGetMonthName(month, MonthNameStyles.LeapYear, (repeatCount == 3));
            }
            // This is in a regular year.
            if (month >= 7) {
                month++;
            }
            if (repeatCount == 3) {
                return dtfi.GetAbbreviatedMonthName(month);
            }
            return dtfi.GetMonthName(month);
        }

        //
        // The pos should point to a quote character. This method will
        // get the string encloed by the quote character.
        //
        internal static int ParseQuoteString(ReadOnlySpan<char> format, int pos, ref ValueStringBuilder result) {
            //
            // NOTE : pos will be the index of the quote character in the 'format' string.
            //
            int formatLen = format.Length;
            int beginPos = pos;
            char quoteChar = format[pos++]; // Get the character used to quote the following string.

            bool foundQuote = false;
            while (pos < formatLen) {
                char ch = format[pos++];
                if (ch == quoteChar) {
                    foundQuote = true;
                    break;
                }
                else if (ch == '\\') {
                    // The following are used to support escaped character.
                    // Escaped character is also supported in the quoted string.
                    // Therefore, someone can use a format like "'minute:' mm\"" to display:
                    //  minute: 45"
                    // because the second double quote is escaped.
                    if (pos < formatLen) {
                        result.Append(format[pos++]);
                    }
                    else {
                        //
                        // This means that '\' is at the end of the formatting string.
                        //
                        throw new FormatException("Invalid Format");
                    }
                }
                else {
                    result.Append(ch);
                }
            }

            if (!foundQuote) {
                // Here we can't find the matching quote.
                throw new FormatException("Invalid Format");
            }

            //
            // Return the character count including the begin/end quote characters and enclosed string.
            //
            return (pos - beginPos);
        }

        //
        // Get the next character at the index of 'pos' in the 'format' string.
        // Return value of -1 means 'pos' is already at the end of the 'format' string.
        // Otherwise, return value is the int value of the next character.
        //
        internal static int ParseNextChar(ReadOnlySpan<char> format, int pos) {
            if (pos >= format.Length - 1) {
                return (-1);
            }
            return format[pos + 1];
        }

        //
        //  IsUseGenitiveForm
        //
        //  Actions: Check the format to see if we should use genitive month in the formatting.
        //      Starting at the position (index) in the (format) string, look back and look ahead to
        //      see if there is "d" or "dd".  In the case like "d MMMM" or "MMMM dd", we can use
        //      genitive form.  Genitive form is not used if there is more than two "d".
        //  Arguments:
        //      format      The format string to be scanned.
        //      index       Where we should start the scanning.  This is generally where "M" starts.
        //      tokenLen    The len of the current pattern character.  This indicates how many "M" that we have.
        //      patternToMatch  The pattern that we want to search. This generally uses "d"
        //
        private static bool IsUseGenitiveForm(ReadOnlySpan<char> format, int index, int tokenLen, char patternToMatch) {
            int i;
            int repeat = 0;
            //
            // Look back to see if we can find "d" or "ddd"
            //

            // Find first "d".
            for (i = index - 1; i >= 0 && format[i] != patternToMatch; i--) {  /*Do nothing here */ };

            if (i >= 0) {
                // Find a "d", so look back to see how many "d" that we can find.
                while (--i >= 0 && format[i] == patternToMatch) {
                    repeat++;
                }
                //
                // repeat == 0 means that we have one (patternToMatch)
                // repeat == 1 means that we have two (patternToMatch)
                //
                if (repeat <= 1) {
                    return (true);
                }
                // Note that we can't just stop here.  We may find "ddd" while looking back, and we have to look
                // ahead to see if there is "d" or "dd".
            }

            //
            // If we can't find "d" or "dd" by looking back, try look ahead.
            //

            // Find first "d"
            for (i = index + tokenLen; i < format.Length && format[i] != patternToMatch; i++) { /* Do nothing here */ };

            if (i < format.Length) {
                repeat = 0;
                // Find a "d", so contine the walk to see how may "d" that we can find.
                while (++i < format.Length && format[i] == patternToMatch) {
                    repeat++;
                }
                //
                // repeat == 0 means that we have one (patternToMatch)
                // repeat == 1 means that we have two (patternToMatch)
                //
                if (repeat <= 1) {
                    return (true);
                }
            }
            return false;
        }

        private static void FormatCustomized(DateTime dateTime, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, TimeSpan offset, ref ValueStringBuilder result) {
            FormatCustomized(dateTime, format, 0, format.Length, dtfi, offset, ref result);
        }

        //
        //  FormatCustomized
        //
        //  Actions: Format the DateTime instance using the specified format.
        //
        private static void FormatCustomized(DateTime dateTime, ReadOnlySpan<char> format, int index, int length, DateTimeFormatInfo dtfi, TimeSpan offset, ref ValueStringBuilder result) {
            Calendar cal = dtfi.Calendar;
            //StringBuilder outputBuffer = StringBuilderCache.Acquire();
            // This is a flag to indicate if we are format the dates using Hebrew calendar.

            bool isHebrewCalendar = cal is HebrewCalendar;//(cal.ID == Calendar.CAL_HEBREW); 
            bool isJapaneseCalendar = cal is JapaneseCalendar;// (cal.ID == Calendar.CAL_JAPAN);

            // This is a flag to indicate if we are formating hour/minute/second only.
            bool bTimeOnly = true;

            int i = index;//0;
            int end = index + length;// format.Length;
            int tokenLen, hour12;

            while (i < end) {
                char ch = format[i];
                int nextChar;
                switch (ch) {
                    case 'g':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(dtfi.GetEraName(cal.GetEra(dateTime)));
                        break;
                    case 'h':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        hour12 = dateTime.Hour % 12;
                        if (hour12 == 0) {
                            hour12 = 12;
                        }
                        FormatDigits(hour12, tokenLen, ref result);
                        break;
                    case 'H':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        FormatDigits(dateTime.Hour, tokenLen, ref result);
                        break;
                    case 'm':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        FormatDigits(dateTime.Minute, tokenLen, ref result);
                        break;
                    case 's':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        FormatDigits(dateTime.Second, tokenLen, ref result);
                        break;
                    case 'f':
                    case 'F':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        if (tokenLen <= MaxSecondsFractionDigits) {
                            long fraction = (dateTime.Ticks % 10000000);//TicksPerSecond
                            fraction = fraction / (long)Math.Pow(10, 7 - tokenLen);
                            if (ch == 'f') {
                                //result.Append(((int)fraction).ToString(fixedNumberFormats[tokenLen - 1], CultureInfo.InvariantCulture));
                                NumberFormatter.NumberToString(fixedNumberFormats[tokenLen - 1], (int)fraction, CultureInfo.InvariantCulture, ref result);
                            }
                            else {
                                int effectiveDigits = tokenLen;
                                while (effectiveDigits > 0) {
                                    if (fraction % 10 == 0) {
                                        fraction = fraction / 10;
                                        effectiveDigits--;
                                    }
                                    else {
                                        break;
                                    }
                                }
                                if (effectiveDigits > 0) {
                                    //result.Append(((int)fraction).ToString(fixedNumberFormats[effectiveDigits - 1], CultureInfo.InvariantCulture));
                                    NumberFormatter.NumberToString(fixedNumberFormats[effectiveDigits - 1], (int)fraction, CultureInfo.InvariantCulture, ref result);
                                }
                                else {
                                    // No fraction to emit, so see if we should remove decimal also.
                                    if (result.Length > 0 && result[result.Length - 1] == '.') {
                                        result.Remove(result.Length - 1, 1);
                                    }
                                }
                            }
                        }
                        else {
                            throw new FormatException("Invalid Format");
                        }
                        break;
                    case 't':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        if (tokenLen == 1) {
                            if (dateTime.Hour < 12) {
                                if (dtfi.AMDesignator.Length >= 1) {
                                    result.Append(dtfi.AMDesignator[0]);
                                }
                            }
                            else {
                                if (dtfi.PMDesignator.Length >= 1) {
                                    result.Append(dtfi.PMDesignator[0]);
                                }
                            }

                        }
                        else {
                            result.Append((dateTime.Hour < 12 ? dtfi.AMDesignator : dtfi.PMDesignator));
                        }
                        break;
                    case 'd':
                        //
                        // tokenLen == 1 : Day of month as digits with no leading zero.
                        // tokenLen == 2 : Day of month as digits with leading zero for single-digit months.
                        // tokenLen == 3 : Day of week as a three-leter abbreviation.
                        // tokenLen >= 4 : Day of week as its full name.
                        //
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        if (tokenLen <= 2) {
                            int day = cal.GetDayOfMonth(dateTime);
                            if (isHebrewCalendar) {
                                // For Hebrew calendar, we need to convert numbers to Hebrew text for yyyy, MM, and dd values.
                                HebrewFormatDigits(day, ref result);
                            }
                            else {
                                FormatDigits(day, tokenLen, ref result);
                            }
                        }
                        else {
                            int dayOfWeek = (int)cal.GetDayOfWeek(dateTime);
                            result.Append(FormatDayOfWeek(dayOfWeek, tokenLen, dtfi));
                        }
                        bTimeOnly = false;
                        break;
                    case 'M':
                        //
                        // tokenLen == 1 : Month as digits with no leading zero.
                        // tokenLen == 2 : Month as digits with leading zero for single-digit months.
                        // tokenLen == 3 : Month as a three-letter abbreviation.
                        // tokenLen >= 4 : Month as its full name.
                        //
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        int month = cal.GetMonth(dateTime);
                        if (tokenLen <= 2) {
                            if (isHebrewCalendar) {
                                // For Hebrew calendar, we need to convert numbers to Hebrew text for yyyy, MM, and dd values.
                                HebrewFormatDigits(month, ref result);
                            }
                            else {
                                FormatDigits(month, tokenLen, ref result);
                            }
                        }
                        else {
                            //throw new FormatException("Unsupported format");
                            if (isHebrewCalendar) {
                                result.Append(FormatHebrewMonthName(dateTime, month, tokenLen, dtfi));
                            }
                            else {
                                //if ((dtfi.FormatFlags & DateTimeFormatFlags.UseGenitiveMonth) != 0 && tokenLen >= 4)
                                //{
                                //    result.Append(
                                //        dtfi.internalGetMonthName(
                                //            month,
                                //            IsUseGenitiveForm(format, i, tokenLen, 'd') ? MonthNameStyles.Genitive : MonthNameStyles.Regular,
                                //            false));
                                //}
                                //else
                                //{
                                //    result.Append(FormatMonth(month, tokenLen, dtfi));
                                //}

                                if (tokenLen >= 4 && IsUseGenitiveForm(format, i, tokenLen, 'd') && UseGenitiveMonth(dtfi)) {
                                    result.Append(
                                        GetMonthName(
                                            dtfi,
                                            month,
                                             1,//MonthNameStyles.Genitive
                                            false));
                                }
                                else {
                                    result.Append(FormatMonth(month, tokenLen, dtfi));
                                }
                            }
                        }
                        bTimeOnly = false;
                        break;
                    case 'y':
                        // Notes about OS behavior:
                        // y: Always print (year % 100). No leading zero.
                        // yy: Always print (year % 100) with leading zero.
                        // yyy/yyyy/yyyyy/... : Print year value.  No leading zero.

                        int year = cal.GetYear(dateTime);
                        tokenLen = ParseRepeatPattern(format, i, ch);

                        if (isJapaneseCalendar &&
                            !FormatJapaneseFirstYearAsANumber() &&
                            year == 1 &&
                            ((i + tokenLen < format.Length && format[i + tokenLen] == CJKYearSuff[0]) ||
                            (i + tokenLen < format.Length - 1 && format[i + tokenLen] == '\'' && format[i + tokenLen + 1] == CJKYearSuff[0]))) {
                            // We are formatting a Japanese date with year equals 1 and the year number is followed by the year sign \u5e74
                            // In Japanese dates, the first year in the era is not formatted as a number 1 instead it is formatted as \u5143 which means
                            // first or beginning of the era.
                            //outputBuffer.Append(DateTimeFormatInfo.JapaneseEraStart[0]);
                            result.Append(JapaneseEraStart[0]);
                        }
                        else if (HasForceTwoDigitYears(cal)) {
                            FormatDigits(year, tokenLen <= 2 ? tokenLen : 2, ref result);
                        }
                        else if (isHebrewCalendar) {
                            HebrewFormatDigits(year, ref result);
                        }
                        else {
                            if (tokenLen <= 2) {
                                FormatDigits(year % 100, tokenLen, ref result);
                            }
                            else {
                                string fmtPattern = tokenLen > 7 ? "D" + tokenLen : fixedNumberFormats[tokenLen - 1];
                                //result.Append(year.ToString(fmtPattern, CultureInfo.InvariantCulture));
                                NumberFormatter.NumberToString(fmtPattern, year, CultureInfo.InvariantCulture, ref result);
                            }
                        }
                        bTimeOnly = false;
                        break;
                    case 'z':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        FormatCustomizedTimeZone(dateTime, offset, format, tokenLen, bTimeOnly, ref result);
                        break;
                    case 'K':
                        tokenLen = 1;
                        FormatCustomizedRoundripTimeZone(dateTime, offset, ref result);
                        break;
                    case ':':
                        result.Append(dtfi.TimeSeparator);
                        tokenLen = 1;
                        break;
                    case '/':
                        result.Append(dtfi.DateSeparator);
                        tokenLen = 1;
                        break;
                    case '\'':
                    case '\"':
                        //StringBuilder enquotedString = new StringBuilder();
                        //tokenLen = ParseQuoteString(format, i, enquotedString);
                        //result.Append(enquotedString);

                        tokenLen = ParseQuoteString(format, i, ref result);
                        break;
                    case '%':
                        // Optional format character.
                        // For example, format string "%d" will print day of month
                        // without leading zero.  Most of the cases, "%" can be ignored.
                        nextChar = ParseNextChar(format, i);
                        // nextChar will be -1 if we already reach the end of the format string.
                        // Besides, we will not allow "%%" appear in the pattern.
                        if (nextChar >= 0 && nextChar != (int)'%') {
                            //result.Append(FormatCustomized(dateTime, ((char)nextChar).ToString(), dtfi, offset));
                            //result.Append(FormatCustomized(dateTime, format, i + 1, 1, dtfi, offset));
                            FormatCustomized(dateTime, format, i + 1, 1, dtfi, offset, ref result);
                            tokenLen = 2;
                        }
                        else {
                            //
                            // This means that '%' is at the end of the format string or
                            // "%%" appears in the format string.
                            //
                            throw new FormatException("Invalid Format");
                        }
                        break;
                    case '\\':
                        // Escaped character.  Can be used to insert character into the format string.
                        // For exmple, "\d" will insert the character 'd' into the string.
                        //
                        // NOTENOTE : we can remove this format character if we enforce the enforced quote
                        // character rule.
                        // That is, we ask everyone to use single quote or double quote to insert characters,
                        // then we can remove this character.
                        //
                        nextChar = ParseNextChar(format, i);
                        if (nextChar >= 0) {
                            result.Append(((char)nextChar));
                            tokenLen = 2;
                        }
                        else {
                            //
                            // This means that '\' is at the end of the formatting string.
                            //
                            throw new FormatException("Invalid Format");
                        }
                        break;
                    default:
                        // NOTENOTE : we can remove this rule if we enforce the enforced quote
                        // character rule.
                        // That is, if we ask everyone to use single quote or double quote to insert characters,
                        // then we can remove this default block.
                        result.Append(ch);
                        tokenLen = 1;
                        break;
                }
                i += tokenLen;
            }
        }

        private static ConcurrentDictionary<DateTimeFormatInfo, DateTimeFormatInfoCache> dateTimeFormatInfoCaches = new ConcurrentDictionary<DateTimeFormatInfo, DateTimeFormatInfoCache>();
        private static DateTimeFormatInfoCache GetDateTimeFormatInfoCache(DateTimeFormatInfo dtfi) {
            return dateTimeFormatInfoCaches.GetOrAdd(dtfi, (key) => new DateTimeFormatInfoCache(key));
        }
        private static bool UseGenitiveMonth(DateTimeFormatInfo dtfi) {
            ////UseGenitiveMonth = 0x00000001,
            DateTimeFormatInfoCache cache = GetDateTimeFormatInfoCache(dtfi);
            return cache.UseGenitiveMonth;
        }

        private static string GetMonthName(DateTimeFormatInfo dtfi, int month, int style, bool abbreviated) {
            DateTimeFormatInfoCache cache = GetDateTimeFormatInfoCache(dtfi);
            string[] monthNamesArray = null;
            switch (style) {
                case 1://1: Genitive
                    monthNamesArray = abbreviated ? cache.AbbreviatedMonthGenitiveNames : cache.MonthGenitiveNames;
                    break;
                case 2://2: LeapYear 
                    monthNamesArray = cache.LeapYearMonthNames;
                    break;
                default://0: Regular
                    monthNamesArray = abbreviated ? cache.AbbreviatedMonthNames : cache.MonthNames;
                    break;
            }

            if ((month < 1) || (month > monthNamesArray.Length))
                throw new ArgumentOutOfRangeException("month");

            return (monthNamesArray[month - 1]);
        }

        private static bool FormatJapaneseFirstYearAsANumber() {
            if (_formatJapaneseFirstYearAsANumber < 0)
                return false;
            if (_formatJapaneseFirstYearAsANumber > 0)
                return true;

            bool isSwitchEnabled;
            AppContext.TryGetSwitch(SwitchFormatJapaneseFirstYearAsANumber, out isSwitchEnabled);
            _formatJapaneseFirstYearAsANumber = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
            return isSwitchEnabled;
        }

        private static bool HasForceTwoDigitYears(Calendar calendar) {
            if (calendar is JapaneseCalendar || calendar is TaiwanCalendar)
                return true;
            return false;
        }


        // output the 'z' famliy of formats, which output a the offset from UTC, e.g. "-07:30"
        private static void FormatCustomizedTimeZone(DateTime dateTime, TimeSpan offset, ReadOnlySpan<char> format, int tokenLen, bool timeOnly, ref ValueStringBuilder result) {
            // See if the instance already has an offset
            bool dateTimeFormat = (offset == NullOffset);
            if (dateTimeFormat) {
                // No offset. The instance is a DateTime and the output should be the local time zone

                if (timeOnly && dateTime.Ticks < TimeSpan.TicksPerDay) {
                    // For time only format and a time only input, the time offset on 0001/01/01 is less
                    // accurate than the system's current offset because of daylight saving time.
                    offset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
                }
                else if (dateTime.Kind == DateTimeKind.Utc) {
#if FEATURE_CORECLR
                                    offset = TimeSpan.Zero;
#else // FEATURE_CORECLR
                    // This code path points to a bug in user code. It would make sense to return a 0 offset in this case.
                    // However, because it was only possible to detect this in Whidbey, there is user code that takes a
                    // dependency on being serialize a UTC DateTime using the 'z' format, and it will work almost all the
                    // time if it is offset by an incorrect conversion to local time when parsed. Therefore, we need to
                    // explicitly emit the local time offset, which we can do by removing the UTC flag.
                    InvalidFormatForUtc(format, dateTime);
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                    offset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
#endif // FEATURE_CORECLR
                }
                else {
                    offset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
                }
            }
            if (offset >= TimeSpan.Zero) {
                result.Append('+');
            }
            else {
                result.Append('-');
                // get a positive offset, so that you don't need a separate code path for the negative numbers.
                offset = offset.Negate();
            }

            if (tokenLen <= 1) {
                // 'z' format e.g "-7"              
                //result.AppendFormat(CultureInfo.InvariantCulture, "{0:0}", offset.Hours);
                NumberFormatter.NumberToString("0", offset.Hours, CultureInfo.InvariantCulture, ref result);
            }
            else {
                // 'zz' or longer format e.g "-07"
                //result.AppendFormat(CultureInfo.InvariantCulture, "{0:00}", offset.Hours);
                NumberFormatter.NumberToString("00", offset.Hours, CultureInfo.InvariantCulture, ref result);
                if (tokenLen >= 3) {
                    // 'zzz*' or longer format e.g "-07:30"
                    //result.AppendFormat(CultureInfo.InvariantCulture, ":{0:00}", offset.Minutes);
                    NumberFormatter.NumberToString("00", offset.Minutes, CultureInfo.InvariantCulture, ref result);
                }
            }
        }

        // output the 'K' format, which is for round-tripping the data
        private static void FormatCustomizedRoundripTimeZone(DateTime dateTime, TimeSpan offset, ref ValueStringBuilder result) {

            // The objective of this format is to round trip the data in the type
            // For DateTime it should round-trip the Kind value and preserve the time zone.
            // DateTimeOffset instance, it should do so by using the internal time zone.

            if (offset == NullOffset) {
                // source is a date time, so behavior depends on the kind.
                switch (dateTime.Kind) {
                    case DateTimeKind.Local:
                        // This should output the local offset, e.g. "-07:30"
                        offset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
                        // fall through to shared time zone output code
                        break;
                    case DateTimeKind.Utc:
                        // The 'Z' constant is a marker for a UTC date
                        result.Append("Z");
                        return;
                    default:
                        // If the kind is unspecified, we output nothing here
                        return;
                }
            }
            if (offset >= TimeSpan.Zero) {
                result.Append('+');
            }
            else {
                result.Append('-');
                // get a positive offset, so that you don't need a separate code path for the negative numbers.
                offset = offset.Negate();
            }

            //result.AppendFormat(CultureInfo.InvariantCulture, "{0:00}:{1:00}", offset.Hours, offset.Minutes);
            NumberFormatter.NumberToString("00", offset.Hours, CultureInfo.InvariantCulture, ref result);
            result.Append(":");
            NumberFormatter.NumberToString("00", offset.Minutes, CultureInfo.InvariantCulture, ref result);
        }


        internal static string GetRealFormat(ReadOnlySpan<char> format, DateTimeFormatInfo dtfi) {
            string realFormat = null;

            switch (format[0]) {
                case 'd':       // Short Date
                    realFormat = dtfi.ShortDatePattern;
                    break;
                case 'D':       // Long Date
                    realFormat = dtfi.LongDatePattern;
                    break;
                case 'f':       // Full (long date + short time)
                    realFormat = GetDateTimeFormatInfoCache(dtfi).LongDateShortTimePattern; //dtfi.LongDatePattern + " " + dtfi.ShortTimePattern;
                    break;
                case 'F':       // Full (long date + long time)
                    realFormat = dtfi.FullDateTimePattern;
                    break;
                case 'g':       // General (short date + short time)
                    realFormat = GetDateTimeFormatInfoCache(dtfi).GeneralShortTimePattern; //dtfi.GeneralShortTimePattern; 
                    break;
                case 'G':       // General (short date + long time)                    
                    realFormat = GetDateTimeFormatInfoCache(dtfi).GeneralLongTimePattern; //dtfi.GeneralLongTimePattern; 
                    break;
                case 'm':
                case 'M':       // Month/Day Date
                    realFormat = dtfi.MonthDayPattern;
                    break;
                case 'o':
                case 'O':
                    realFormat = RoundtripFormat;
                    break;
                case 'r':
                case 'R':       // RFC 1123 Standard
                    realFormat = dtfi.RFC1123Pattern;
                    break;
                case 's':       // Sortable without Time Zone Info
                    realFormat = dtfi.SortableDateTimePattern;
                    break;
                case 't':       // Short Time
                    realFormat = dtfi.ShortTimePattern;
                    break;
                case 'T':       // Long Time
                    realFormat = dtfi.LongTimePattern;
                    break;
                case 'u':       // Universal with Sortable format
                    realFormat = dtfi.UniversalSortableDateTimePattern;
                    break;
                case 'U':       // Universal with Full (long date + long time) format
                    realFormat = dtfi.FullDateTimePattern;
                    break;
                case 'y':
                case 'Y':       // Year/Month Date
                    realFormat = dtfi.YearMonthPattern;
                    break;
                default:
                    throw new FormatException("Invalid Format");
            }
            return realFormat;
        }


        // Expand a pre-defined format string (like "D" for long date) to the real format that
        // we are going to use in the date time parsing.
        // This method also convert the dateTime if necessary (e.g. when the format is in Universal time),
        // and change dtfi if necessary (e.g. when the format should use invariant culture).
        //
        private static ReadOnlySpan<char> ExpandPredefinedFormat(ReadOnlySpan<char> format, ref DateTime dateTime, ref DateTimeFormatInfo dtfi, ref TimeSpan offset) {
            switch (format[0]) {
                case 's':       // Sortable without Time Zone Info
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    break;
                case 'u':       // Universal time in sortable format.
                    if (offset != NullOffset) {
                        // Convert to UTC invariants mean this will be in range
                        dateTime = dateTime - offset;
                    }
                    else if (dateTime.Kind == DateTimeKind.Local) {

                        InvalidFormatForLocal(format, dateTime);
                    }
                    dtfi = DateTimeFormatInfo.InvariantInfo;
                    break;
                case 'U':       // Universal time in culture dependent format.
                    if (offset != NullOffset) {
                        // This format is not supported by DateTimeOffset
                        throw new FormatException($"This format \"{format.ToString()}\" is not supported by DateTimeOffset");
                    }
                    // Universal time is always in Greogrian calendar.
                    //
                    // Change the Calendar to be Gregorian Calendar.
                    //
                    dtfi = (DateTimeFormatInfo)dtfi.Clone();
                    if (dtfi.Calendar.GetType() != typeof(GregorianCalendar)) {
                        dtfi.Calendar = DEFAULT_GREGORIAN_CALENDAR;// GregorianCalendar.GetDefaultInstance();
                    }
                    dateTime = dateTime.ToUniversalTime();
                    break;
            }
            return GetRealFormat(format, dtfi);
        }

        internal static void Format(DateTime dateTime, ReadOnlySpan<char> format, ref ValueStringBuilder result) {
            Format(dateTime, format, DateTimeFormatInfo.GetInstance(null), NullOffset, ref result);
        }

        internal static void Format(DateTime dateTime, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, ref ValueStringBuilder result) {
            Format(dateTime, format, dtfi, NullOffset, ref result);
        }

        internal static void Format(DateTime dateTime, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, TimeSpan offset, ref ValueStringBuilder result) {
            Contract.Requires(dtfi != null);
            if (format == null || format.Length == 0) {
                bool timeOnlySpecialCase = false;
                if (dateTime.Ticks < TimeSpan.TicksPerDay) {
                    // If the time is less than 1 day, consider it as time of day.
                    // Just print out the short time format.
                    //
                    // This is a workaround for VB, since they use ticks less then one day to be
                    // time of day.  In cultures which use calendar other than Gregorian calendar, these
                    // alternative calendar may not support ticks less than a day.
                    // For example, Japanese calendar only supports date after 1868/9/8.
                    // This will pose a problem when people in VB get the time of day, and use it
                    // to call ToString(), which will use the general format (short date + long time).
                    // Since Japanese calendar does not support Gregorian year 0001, an exception will be
                    // thrown when we try to get the Japanese year for Gregorian year 0001.
                    // Therefore, the workaround allows them to call ToString() for time of day from a DateTime by
                    // formatting as ISO 8601 format.                      
                    //switch (dtfi.Calendar.ID)
                    //{
                    //    case Calendar.CAL_JAPAN:
                    //    case Calendar.CAL_TAIWAN:
                    //    case Calendar.CAL_HIJRI:
                    //    case Calendar.CAL_HEBREW:
                    //    case Calendar.CAL_JULIAN:
                    //    case Calendar.CAL_UMALQURA:
                    //    case Calendar.CAL_PERSIAN:
                    //        timeOnlySpecialCase = true;
                    //        dtfi = DateTimeFormatInfo.InvariantInfo;
                    //        break;
                    //}
                    Calendar cal = dtfi.Calendar;
                    if (cal is JapaneseCalendar || cal is TaiwanCalendar || cal is HijriCalendar || cal is HebrewCalendar || cal is JulianCalendar || cal is UmAlQuraCalendar || cal is PersianCalendar) {
                        timeOnlySpecialCase = true;
                        dtfi = DateTimeFormatInfo.InvariantInfo;
                    }
                }
                if (offset == NullOffset) {
                    // Default DateTime.ToString case.
                    if (timeOnlySpecialCase) {
                        format = "s";
                    }
                    else {
                        format = "G";
                    }
                }
                else {
                    // Default DateTimeOffset.ToString case.
                    if (timeOnlySpecialCase) {
                        format = RoundtripDateTimeUnfixed;
                    }
                    else {
                        format = GetDateTimeFormatInfoCache(dtfi).DateTimeOffsetPattern;// dtfi.DateTimeOffsetPattern;
                    }
                }

            }

            if (format.Length == 1) {
                switch (format[0]) {
                    case 'O':
                    case 'o': {
                            FastFormatRoundtrip(dateTime, offset, ref result);
                            return;
                        }
                    case 'R':
                    case 'r': {
                            FastFormatRfc1123(dateTime, offset, dtfi, ref result);
                            return;
                        }
                }

                format = ExpandPredefinedFormat(format, ref dateTime, ref dtfi, ref offset);
            }

            FormatCustomized(dateTime, format, dtfi, offset, ref result);
        }



        internal static void FastFormatRfc1123(DateTime dateTime, TimeSpan offset, DateTimeFormatInfo dtfi, ref ValueStringBuilder result) {
            // ddd, dd MMM yyyy HH:mm:ss GMT
            //const int Rfc1123FormatLength = 29;
            //StringBuilder result = StringBuilderCache.Acquire(Rfc1123FormatLength);

            if (offset != NullOffset) {
                // Convert to UTC invariants
                dateTime = dateTime - offset;
            }

            result.Append(InvariantAbbreviatedDayNames[(int)dateTime.DayOfWeek]);
            result.Append(',');
            result.Append(' ');
            AppendNumber(dateTime.Day, 2, ref result);
            result.Append(' ');
            result.Append(InvariantAbbreviatedMonthNames[dateTime.Month - 1]);
            result.Append(' ');
            AppendNumber(dateTime.Year, 4, ref result);
            result.Append(' ');
            AppendHHmmssTimeOfDay(dateTime, ref result);
            result.Append(' ');
            result.Append(Gmt);
            //return result;
        }

        internal static void FastFormatRoundtrip(DateTime dateTime, TimeSpan offset, ref ValueStringBuilder result) {
            // yyyy-MM-ddTHH:mm:ss.fffffffK
            //const int roundTripFormatLength = 28;
            //StringBuilder result = StringBuilderCache.Acquire(roundTripFormatLength);

            AppendNumber(dateTime.Year, 4, ref result);
            result.Append('-');
            AppendNumber(dateTime.Month, 2, ref result);
            result.Append('-');
            AppendNumber(dateTime.Day, 2, ref result);
            result.Append('T');
            AppendHHmmssTimeOfDay(dateTime, ref result);
            result.Append('.');

            long fraction = dateTime.Ticks % TimeSpan.TicksPerSecond;
            AppendNumber(fraction, 7, ref result);

            FormatCustomizedRoundripTimeZone(dateTime, offset, ref result);
            //return result;
        }

        private static void AppendHHmmssTimeOfDay(DateTime dateTime, ref ValueStringBuilder result) {
            // HH:mm:ss
            AppendNumber(dateTime.Hour, 2, ref result);
            result.Append(':');
            AppendNumber(dateTime.Minute, 2, ref result);
            result.Append(':');
            AppendNumber(dateTime.Second, 2, ref result);
        }

        internal static void AppendNumber(long val, int digits, ref ValueStringBuilder result) {
            for (int i = 0; i < digits; i++) {
                result.Append('0');
            }

            int index = 1;
            while (val > 0 && index <= digits) {
                result[result.Length - index] = (char)('0' + (val % 10));
                val = val / 10;
                index++;
            }

            //BCLDebug.Assert(val == 0, "DateTimeFormat.AppendNumber(): digits less than size of val");
            Contract.Assert(val == 0, "DateTimeFormat.AppendNumber(): digits less than size of val");
        }

        //internal static string[] GetAllDateTimes(DateTime dateTime, char format, DateTimeFormatInfo dtfi)
        //{
        //    Contract.Requires(dtfi != null);
        //    string[] allFormats = null;
        //    string[] results = null;

        //    switch (format)
        //    {
        //        case 'd':
        //        case 'D':
        //        case 'f':
        //        case 'F':
        //        case 'g':
        //        case 'G':
        //        case 'm':
        //        case 'M':
        //        case 't':
        //        case 'T':
        //        case 'y':
        //        case 'Y':
        //            allFormats = dtfi.GetAllDateTimePatterns(format);
        //            results = new String[allFormats.Length];
        //            for (int i = 0; i < allFormats.Length; i++)
        //            {
        //                results[i] = Format(dateTime, allFormats[i], dtfi);
        //            }
        //            break;
        //        case 'U':
        //            DateTime universalTime = dateTime.ToUniversalTime();
        //            allFormats = dtfi.GetAllDateTimePatterns(format);
        //            results = new String[allFormats.Length];
        //            for (int i = 0; i < allFormats.Length; i++)
        //            {
        //                results[i] = Format(universalTime, allFormats[i], dtfi);
        //            }
        //            break;
        //        //
        //        // The following ones are special cases because these patterns are read-only in
        //        // DateTimeFormatInfo.
        //        //
        //        case 'r':
        //        case 'R':
        //        case 'o':
        //        case 'O':
        //        case 's':
        //        case 'u':
        //            results = new string[] { Format(dateTime, new string(new char[] { format }), dtfi) };
        //            break;
        //        default:
        //            throw new FormatException($"Invalid Format:{format}");

        //    }
        //    return (results);
        //}

        //internal static string[] GetAllDateTimes(DateTime dateTime, DateTimeFormatInfo dtfi)
        //{
        //    List<string> results = new List<string>(DEFAULT_ALL_DATETIMES_SIZE);

        //    for (int i = 0; i < allStandardFormats.Length; i++)
        //    {
        //        string[] strings = GetAllDateTimes(dateTime, allStandardFormats[i], dtfi);
        //        for (int j = 0; j < strings.Length; j++)
        //        {
        //            results.Add(strings[j]);
        //        }
        //    }
        //    string[] value = new string[results.Count];
        //    results.CopyTo(0, value, 0, results.Count);
        //    return (value);
        //}

        // This is a placeholder for an MDA to detect when the user is using a
        // local DateTime with a format that will be interpreted as UTC.
        internal static void InvalidFormatForLocal(ReadOnlySpan<char> format, DateTime dateTime) {
        }

        // This is an MDA for cases when the user is using a local format with
        // a Utc DateTime.
        [System.Security.SecuritySafeCritical]  // auto-generated
        internal static void InvalidFormatForUtc(ReadOnlySpan<char> format, DateTime dateTime) {
#if MDA_SUPPORTED
                    Mda.DateTimeInvalidLocalFormat();
#endif
        }

        private class DateTimeFormatInfoCache {
            private readonly DateTimeFormatInfo dtfi;
            private string[] monthGenitiveNames;
            private string[] abbreviatedMonthGenitiveNames;
            private string[] monthNames;
            private string[] abbreviatedMonthNames;
            private string[] leapYearMonthNames;
            private string longDateShortTimePattern;
            private string generalShortTimePattern;
            private string generalLongTimePattern;
            private string dateTimeOffsetPattern;
            private int formatFlags = -1;

            public DateTimeFormatInfoCache(DateTimeFormatInfo dtfi) {
                this.dtfi = dtfi;
            }

            private int FormatFlags {
                get {
                    if (formatFlags == -1) {
                        try {
                            PropertyInfo property = typeof(DateTimeFormatInfo).GetProperty("FormatFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                            formatFlags = (int)property.GetValue(dtfi);
                        }
                        catch (Exception e) {
                            formatFlags = 0;
                            //if (LOG.IsWarnEnabled)
                            //    LOG.WarnFormat("Failed to obtain DateTimeFormatInfo.FormatFlags using reflection, use default value instead of this property,Exception:{0}", e);
                        }
                    }
                    return formatFlags;
                }
            }
            public bool UseGenitiveMonth { get { return (FormatFlags & 0x1) != 0; } }

            public string[] LeapYearMonthNames {
                get {
                    if (leapYearMonthNames == null) {
                        try {
                            MethodInfo method = typeof(DateTimeFormatInfo).GetMethod("internalGetLeapYearMonthNames", BindingFlags.NonPublic | BindingFlags.Instance);
                            leapYearMonthNames = (string[])method.Invoke(dtfi, null);
                        }
                        catch (Exception e) {
                            leapYearMonthNames = this.MonthNames;
                            //if (LOG.IsWarnEnabled)
                            //    LOG.WarnFormat("Calling DateTimeFormatInfo.internalGetLeapYearMonthNames() method using reflection fails,Exception:{0}", e);
                        }
                    }
                    return leapYearMonthNames;
                }
            }

            public string[] MonthGenitiveNames {
                get {
                    if (monthGenitiveNames == null)
                        monthGenitiveNames = dtfi.MonthGenitiveNames;
                    return monthGenitiveNames;
                }
            }

            public string[] AbbreviatedMonthGenitiveNames {
                get {
                    if (abbreviatedMonthGenitiveNames == null)
                        abbreviatedMonthGenitiveNames = dtfi.AbbreviatedMonthGenitiveNames;
                    return abbreviatedMonthGenitiveNames;
                }
            }

            public string[] MonthNames {
                get {
                    if (monthNames == null)
                        monthNames = dtfi.MonthNames;
                    return monthNames;
                }
            }

            public string[] AbbreviatedMonthNames {
                get {
                    if (abbreviatedMonthNames == null)
                        abbreviatedMonthNames = dtfi.AbbreviatedMonthNames;
                    return abbreviatedMonthNames;
                }
            }

            public string LongDateShortTimePattern {
                get {
                    if (longDateShortTimePattern == null)
                        longDateShortTimePattern = dtfi.LongDatePattern + " " + dtfi.ShortTimePattern;
                    return longDateShortTimePattern;
                }
            }

            public string GeneralShortTimePattern {
                get {
                    if (generalShortTimePattern == null)
                        generalShortTimePattern = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;//dtfi.GeneralShortTimePattern; 
                    return generalShortTimePattern;
                }
            }

            public string GeneralLongTimePattern {
                get {
                    if (generalLongTimePattern == null)
                        generalLongTimePattern = dtfi.ShortDatePattern + " " + dtfi.LongTimePattern;//dtfi.GeneralLongTimePattern; 
                    return generalLongTimePattern;
                }
            }

            public string DateTimeOffsetPattern {
                get {
                    if (dateTimeOffsetPattern == null) {
                        dateTimeOffsetPattern = dtfi.ShortDatePattern + " " + dtfi.LongTimePattern;

                        /* LongTimePattern might contain a "z" as part of the format string in which case we don't want to append a time zone offset */

                        bool foundZ = false;
                        bool inQuote = false;
                        char quote = '\'';
                        string longTimePattern = dtfi.LongTimePattern;
                        for (int i = 0; !foundZ && i < longTimePattern.Length; i++) {
                            switch (longTimePattern[i]) {
                                case 'z':
                                    /* if we aren't in a quote, we've found a z */
                                    foundZ = !inQuote;
                                    /* we'll fall out of the loop now because the test includes !foundZ */
                                    break;
                                case '\'':
                                case '\"':
                                    if (inQuote && (quote == longTimePattern[i])) {
                                        /* we were in a quote and found a matching exit quote, so we are outside a quote now */
                                        inQuote = false;
                                    }
                                    else if (!inQuote) {
                                        quote = longTimePattern[i];
                                        inQuote = true;
                                    }
                                    else {
                                        /* we were in a quote and saw the other type of quote character, so we are still in a quote */
                                    }
                                    break;
                                case '%':
                                case '\\':
                                    i++; /* skip next character that is escaped by this backslash */
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (!foundZ) {
                            dateTimeOffsetPattern = dateTimeOffsetPattern + " zzz";
                        }
                    }
                    return dateTimeOffsetPattern;
                }
            }
        }
    }
}