using System;
using System.Globalization;

public struct Date : IComparable<DateTime>, IEquatable<DateTime>, IComparable<Date>, IEquatable<Date>, IFormattable
{
    private DateTime date;

    public Date(DateTime date)
    {
        this.date = date.Date;
    }

    public int CompareTo(DateTime other)
    {
        return date.CompareTo(other);
    }

    public int CompareTo(Date other)
    {
        return date.CompareTo(other.date);
    }

    public bool Equals(Date other)
    {
        return date == other.date;
    }

    public bool Equals(DateTime other)
    {
        return date == other;
    }

    public DateTime ToDateTime()
    {
        return date;
    }

    public override bool Equals(object obj)
    {
        return date.Equals(((Date)obj).date);
    }

    public override int GetHashCode()
    {
        return date.GetHashCode();
    }

    public override string ToString()
    {
        return date.ToString();
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return date.ToString(format, formatProvider);
    }

    public static Date Parse(string value)
    {
        return new Date(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal).Date);
    }

    public static Date Parse(string value, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
    {
        return new Date(DateTime.Parse(value, formatProvider, dateTimeStyles).Date);
    }

    public static Date ParseExact(string value, string format, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
    {
        return new Date(DateTime.ParseExact(value, format, formatProvider, dateTimeStyles));
    }

    public static explicit operator Date(DateTime other)
    {
        var date = new Date
        {
            date = other.Date
        };
        return date;
    }

    public static implicit operator DateTime(Date date)
    {
        return date.date;
    }

    public static bool operator ==(Date first, Date second)
    {
        return first.date == second.date;
    }

    public static bool operator !=(Date first, Date second)
    {
        return first.date != second.date;
    }

    public static bool operator >(Date first, Date second)
    {
        return first.date > second.date;
    }

    public static bool operator <(Date first, Date second)
    {
        return first.date < second.date;
    }
}
