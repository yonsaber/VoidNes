namespace VoidNes.Emulator;

/// <summary>
/// Representation of a NES Cartridge based on the NES 2.0 ROM format
/// </summary>
/// <remarks>
/// https://www.nesdev.org/wiki/NES_2.0
/// </remarks>
public class Cartridge
{
    /// <summary>
    /// Program ROM
    /// </summary>
    byte[] _prgRom;

    /// <summary>
    /// Character ROM
    /// </summary>
    byte[] _chrRom;

    /// <summary>
    /// Gets the number of 16KB PRG ROM banks present in this cartridge.
    /// </summary>
    /// <value>The number of 16 KB PRG ROM banks present in this cartridge.</value>
    public int PrgRomBanks { get; private set; }

    /// <summary>
    /// Gets the number of 8KB CHR banks present in this cartridge.
    /// </summary>
    /// <value>The number of 8KB CHR banks present in this cartridge.</value>
    public int ChrBanks { get; private set; }

    /// <summary>
    /// Is <c>true</c> if the Vertical VRAM mirroring iNES flag is set on
    /// on this cartridge.
    /// </summary>
    /// <value><c>true</c> if the vertical VRAM mirroring flag is set; otherwise, <c>false</c>.</value>
    public NameTableMirroring Mirroring { get; private set; }

    /// <summary>
    /// Is <c>true</c> if this cartridge contains battery backed persistent memory.
    /// </summary>
    /// <value><c>true</c> if battery backed memory present; otherwise, <c>false</c>.</value>
    public bool BatteryBackedMemory { get; private set; }

    /// <summary>
    /// Is <c>true</c> if this cartridge contains a 512 byte trainer.
    /// </summary>
    /// <value><c>true</c> if this cartridge contains a trainer; otherwise, <c>false</c>.</value>
    public bool ContainsTrainer { get; private set; }

    /// <summary>
    /// Is true if this cartridge uses CHR RAM as opposed to CHR ROM.
    /// </summary>
    /// <value><c>true</c> if this cartridge uses CHR RAM; otherwise, <c>false</c>.</value>
    public bool UsesChrRam { get; private set; }

    /// <summary>
    /// Gets the iNES mapper number set on this cartridge.
    /// </summary>
    /// <value>This cartridge's iNES mapper number.</value>
    public int MapperNumber { get; private set; }

    int _flags6;
    int _flags7;

    public Cartridge(string romFile)
    {
        ParseRom(romFile);
    }

    private void ParseRom(string romFile)
    {
        var stream = new FileStream(romFile, FileMode.Open, FileAccess.Read);
        var reader = new BinaryReader(stream);

        var header = reader.ReadChars(4);

        // TODO: Get a nice helper function for this
        // 0x4E45531A
        if (header[0] != 'N' && header[1] != 'E' && header[2] != 'S' && header[3] != 0x1A)
            throw new Exception("Not a valid NES ROM!");

        // Size of PRG ROM
        PrgRomBanks = reader.ReadByte();
        Console.WriteLine((16 * PrgRomBanks).ToString() + "Kb of PRG ROM");

        // Size of CHR ROM (Or set CHR RAM if using it)
        ChrBanks = reader.ReadByte();
        if (ChrBanks == 0)
        {
            Console.WriteLine("Cartridge uses CHR RAM");
            ChrBanks = 2;
            UsesChrRam = true;
        }
        else
        {
            Console.WriteLine((8 * ChrBanks).ToString() + "Kb of CHR ROM");
            UsesChrRam = false;
        }

        // Flags 6
        //  7654 3210
        //  ---------
        //  NNNN FTBM
        //  |||| |||+-- Hard-wired nametable mirroring type
        //  |||| |||     0: Horizontal or mapper-controlled
        //  |||| |||     1: Vertical
        //  |||| ||+--- "Battery" and other non-volatile memory
        //  |||| ||      0: Not present
        //  |||| ||      1: Present
        //  |||| |+--- 512-byte Trainer
        //  |||| |      0: Not present
        //  |||| |      1: Present between Header and PRG-ROM data
        //  |||| +---- Hard-wired four-screen mode
        //  ||||        0: No
        //  ||||        1: Yes
        //  ++++------ Mapper Number D0..D3
        _flags6 = reader.ReadByte();
        Mirroring = (NameTableMirroring)(_flags6 & 0x01);
        Console.WriteLine($"VRAM mirroring type: {Mirroring}");

        BatteryBackedMemory = (_flags6 & 0x2) != 0;
        if (BatteryBackedMemory)
            Console.WriteLine("Cartridge contains battery backed persistent memory");

        ContainsTrainer = (_flags6 & 0x04) != 0;
        if (ContainsTrainer)
            Console.WriteLine("Cartridge contains a 512 byte trainer");

        // Flags 7
        //  7654 3210
        //  ---------
        //  NNNN 10TT
        //  |||| ||++- Console type
        //  |||| ||     0: Nintendo Entertainment System/Family Computer
        //  |||| ||     1: Nintendo Vs. System
        //  |||| ||     2: Nintendo Playchoice 10
        //  |||| ||     3: Extended Console Type
        //  |||| ++--- NES 2.0 identifier
        //  ++++------ Mapper Number D4..D7
        _flags7 = reader.ReadByte();

        // Mapper Number
        // We offset by 4 on flags 6 because we only care about the mapper info
        // We use 0xF0 to get us the data we need (the mapper data)
        MapperNumber = _flags7 & 0xF0 | _flags6 >> 4 & 0xF;
    }
}

public enum NameTableMirroring
{
    Unknown = -1,
    Horizontal = 0,
    Vertical = 1
}