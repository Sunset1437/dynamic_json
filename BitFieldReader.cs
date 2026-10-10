using System;

namespace dynamic_json;

public static class BitFieldReader
{
    public static ulong Extract(byte[] payload, int startBit, int stopBit)
    {
        if (startBit < 0 || stopBit < startBit)
            throw new ArgumentException($"Некорректный диапазон бит {startBit}..{stopBit}");

        int bitCount = stopBit - startBit + 1;
        if (bitCount > 64) 
            throw new NotSupportedException($"Поля длиннее 64 бит не поддерживаются (получено {bitCount})");

        int startByte = startBit / 8;
        int endByte   = stopBit / 8;

        if (endByte >= payload.Length)
            throw new InvalidOperationException($"Поле {startBit}..{stopBit} выходит за границы payload " + $"({payload.Length} байт)");

        // Собираем байты в ulong little-endian(?)
        ulong acc = 0;
        int n = endByte - startByte + 1;
        for (int i = 0; i < n; i++)
            acc |= (ulong)payload[startByte + i] << (8 * i);

        int shift = startBit - startByte * 8;
        ulong mask = bitCount == 64 ? ulong.MaxValue : ((1UL << bitCount) - 1);
        return (acc >> shift) & mask;
    }
}