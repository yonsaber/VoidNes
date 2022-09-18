namespace VoidNes.Emulator;

/// <summary>
/// Represents a MOS Technologies 6502 CPU with Decimal Mode disabled (as
/// is the case with the NES).
/// </summary>
public class Cpu
{
    public NesSystem System;

    // Registers
    public byte A; // Accumulator
    public byte X;
    public byte Y;
    public byte S; // Stack Pointer
    public ushort PC; // Program Counter

    // Flags
    bool _carry;
    bool _zero;
    bool _interruptDisable;
    bool _decimal;
    bool _b; // Not Used
    bool _overflow;
    bool _negative;

    public Cpu(NesSystem system)
    {
        // https://www.nesdev.org/wiki/CPU_power_up_state
        _carry = false;
        _zero = false;
        _interruptDisable = true;
        _decimal = false;
        _overflow = false;
        _negative = false;

        A = 0;
        X = 0;
        Y = 0;
        S = 0xFD;
        PC = 0; // Not exactly correct

        System = system;

        var resetVector = System.ReadWord(0xFFC);
        PC = resetVector;
    }
}
