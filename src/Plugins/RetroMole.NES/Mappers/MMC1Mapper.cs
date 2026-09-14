namespace RetroMole.NES.Mappers;

using RetroMole.Core.Mappers;

public class MMC1Mapper : IAddressMapper
{
    public int ToLinearOffset(int bank, int address)
    {
        return (bank << 16) | address;
    }

    public bool TryFromLinearOffset(int linearOffset, out (int bank, int address) result)
    {
        result = (linearOffset >> 16, linearOffset & 0xFFFF);
        return true;
    }
}
