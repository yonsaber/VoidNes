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

    readonly PictureProcessingUnit _pictureProcessingUnit;

    readonly AudioProcessingUnit _audioProcessingUnit;

    public NesSystem()
    {
        // Address range 0x000 to 0x07FF
        _scratchRam = new byte[0x800];

        _pictureProcessingUnit = new PictureProcessingUnit();
        _audioProcessingUnit = new AudioProcessingUnit();
    }

    public byte ReadByte(short address)
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

    public void WriteByte(short address, byte value)
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

    public byte MapperReadByte(short address)
    {
        return 0;
    }

    public void MapperWriteByte(short address, byte value)
    {
        // 
    }
}
