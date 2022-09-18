namespace VoidNes.Emulator;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// How the memrory map is: https://www.nesdev.org/wiki/CPU_memory_map
/// </remarks>
// TODO: Possibly move the memory out of the system ?
public class NesSystem
{
    /// <summary>
    /// 2kb of internal RAM on the NES
    /// </summary>
    readonly byte[] _scratchRam;

    readonly Cpu _cpu;
    
    readonly Cartridge _cartridge;

    readonly PictureProcessingUnit _pictureProcessingUnit;

    readonly AudioProcessingUnit _audioProcessingUnit;

    public NesSystem(string romPath)
    {
        try
        {
            _cartridge = new Cartridge(romPath);
            _cpu = new Cpu(this);

            // Address range 0x000 to 0x07FF
            _scratchRam = new byte[0x800];

            _pictureProcessingUnit = new PictureProcessingUnit();
            _audioProcessingUnit = new AudioProcessingUnit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public byte ReadByte(ushort address)
    {
        // NOTE: Keep within the bounds of the 2KB of scratch ram that we have
        //       but we also have 3 mirrors of the the internal RAM
        // TODO: Understand this better, read the NES wiki
        if (address < 0x2000)
            return _scratchRam[address & 0x7ff];
        else if (address < 0x4000)
            return _pictureProcessingUnit.ReadAddress(address);
        else if (address < 0x4020)
            return _audioProcessingUnit.ReadAddress(address);
        else
            MapperReadByte(address);

        return 0;
    }

    public ushort ReadWord(ushort address)
    {
        ushort output = 0;
        // NOTE: C# autoconverts any summing into int32...
        output += ReadByte((ushort)(address + 1));
        output = (ushort)(output << 8);
        output += ReadByte(address);

        return output;
    }

    public void WriteByte(ushort address, byte value)
    {
        if (address < 0x2000)
            _scratchRam[address & 0x7ff] = value;
        else if (address < 0x4000)
            _pictureProcessingUnit.WriteAddress(address, value);
        else if (address < 0x4020)
            _audioProcessingUnit.WriteAddress(address, value);
        else
            MapperWriteByte(address, value);
    }

    public byte MapperReadByte(ushort address)
    {
        return 0;
    }

    public void MapperWriteByte(ushort address, byte value)
    {
        // 
    }
}
