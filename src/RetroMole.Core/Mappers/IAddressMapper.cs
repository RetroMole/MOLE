namespace RetroMole.Core.Mappers;

public interface IAddressMapper
{
    int ToLinearOffset(int bank, int address);

    bool TryFromLinearOffset(int linearOffset, out (int bank, int address) result);
}
