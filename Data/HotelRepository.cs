public class Hotelrepository : IHotelRepository {
    private readonly HotelDb _context;
    public Hotelrepository(HotelDb context) => _context = context;

    public Task<List<Hotel>> GetHotelsAsync() => _context.Hotels.ToListAsync();

    public async Task<Hotel> GetHotelAsync(int hotelId) => 
        await _context.Hotels.FindAsync(new object[] {hotelId});

    public async Task InsertHotelAsync(Hotel hotel) => await _context.Hotels.AddAsync(hotel);

    public async Task UpdateHotelAsync(Hotel hotel) {
        var hotelFromDb = await _context.Hotels.FindAsync(new object[] {hotel.Id});
        if (hotelFromDb == null) return;
        hotelFromDb.Latitude = hotel.Latitude;
        hotelFromDb.Longitude = hotel.Longitude;
        hotelFromDb.Name = hotel.Name;
    }

    public async Task DeleteHotelAsync(int hotelId) {
        var hotelFromDb = await _context.Hotels.FindAsync(new object[] {hotelId});
        if (hotelFromDb == null) return;
        _context.Hotels.Remove(hotelFromDb);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    private bool _dispose = false;
    protected virtual void Dispose(bool disposing) {
        if (!_dispose) {
            if (disposing) {
                _context.Dispose();
            }
        }
        _dispose = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}