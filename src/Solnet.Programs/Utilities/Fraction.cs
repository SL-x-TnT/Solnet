using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Utilities
{
     public class I80F48Fraction
    {
        private readonly ulong _integerPart; // 80 bits for the integer part
        private readonly ulong _fractionalPart; // 48 bits for the fractional part

        public I80F48Fraction(ulong integerPart, ulong fractionalPart)
        {
            if (fractionalPart >= (1UL << 48))
            {
                throw new ArgumentOutOfRangeException(nameof(fractionalPart), "Fractional part must be less than 2^48.");
            }

            _integerPart = integerPart;
            _fractionalPart = fractionalPart;
        }

        // New constructor that takes a BigInteger
        public I80F48Fraction(BigInteger value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be non-negative.");
            }

            // Extract the integer and fractional parts from the BigInteger
            _integerPart = (ulong)(value >> 48); // High 80 bits for the integer part
            _fractionalPart = (ulong)(value & ((1UL << 48) - 1)); // Low 48 bits for the fractional part

            if (_fractionalPart >= (1UL << 48))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Fractional part must be less than 2^48.");
            }
        }
        
        public static I80F48Fraction operator +(I80F48Fraction a, I80F48Fraction b)
        {
            ulong newFractionalPart = a._fractionalPart + b._fractionalPart;
            ulong newIntegerPart = a._integerPart + b._integerPart;

            // Handle overflow from fractional part to integer part
            if (newFractionalPart >= (1UL << 48))
            {
                newFractionalPart -= (1UL << 48);
                newIntegerPart++;
            }

            return new I80F48Fraction(newIntegerPart, newFractionalPart);
        }

        public static I80F48Fraction operator -(I80F48Fraction a, I80F48Fraction b)
        {
            ulong newFractionalPart = a._fractionalPart - b._fractionalPart;
            ulong newIntegerPart = a._integerPart - b._integerPart;

            // Handle underflow from fractional part to integer part
            if (newFractionalPart < 0)
            {
                newFractionalPart += (1UL << 48);
                newIntegerPart--;
            }

            return new I80F48Fraction(newIntegerPart, newFractionalPart);
        }

        public decimal ToDecimal()
        {
            return (decimal)_integerPart + (decimal)_fractionalPart / (1UL << 48);
        }

        public override string ToString()
        {
            return ToDecimal().ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is I80F48Fraction other)
            {
                return _integerPart == other._integerPart && _fractionalPart == other._fractionalPart;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_integerPart, _fractionalPart);
        }
    }
}
