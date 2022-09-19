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

    public ulong Clock;

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

        System = system;

        Clock = 0;

        var resetVector = System.ReadWord(0xFFC);
        PC = resetVector;
    }

    #region CPU Instruction Set

    // LoaD Accumulator -- retrieves a copy from the specified RAM or I/O address, and stores it in the accumulator
    public void lda(byte opcode)
    {
        // TODO: Possibly look into doing a yield return
        byte intermediate = 0;
        switch (opcode)
        {
            case 0xa9:
                Clock += 2;
                intermediate = System.ReadByte((ushort)(PC + 1));
                break;
            default:
                Console.WriteLine($"Unknown opcode {opcode}");
                break;
        }

        _negative = (intermediate & 0x80) == 0x80;
        _zero = intermediate == 0;

        A = intermediate;
    }

    #endregion
}
