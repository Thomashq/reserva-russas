using Microsoft.Extensions.DependencyInjection;
using RR.Core.Entities;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Seeding;

public static class RoomSeeder
{
    public static async Task SeedRoomsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();

        if (db.Rooms.Any()) return;

        var managers = db.Manager.ToList();
        if (managers.Count == 0) return; // precisa ter manager antes

        // 1) garantir equipamentos base
        var equipments = EnsureDefaultEquipments(db);
        await db.SaveChangesAsync();

        // 2) criar salas (cada uma com um Manager)
        var roomsToCreate = RoomList.ReturnRoomList();

        for (int i = 0; i < roomsToCreate.Count; i++)
        {
            var r = roomsToCreate[i];
            var manager = managers[i % managers.Count];

            var room = new Rooms
            {
                Name = r.Name,
                Capacity = r.Capacity,
                ManagerId = manager.Id,
                IsActive = r.IsActive
            };

            db.Rooms.Add(room);
            await db.SaveChangesAsync(); // precisa do room.Id pro RoomDetails

            var details = new RoomDetails
            {
                RoomId = room.Id,
                IsReserveable = r.IsReserveable,
                RoomType = r.RoomType
            };

            db.RoomDetails.Add(details);
            await db.SaveChangesAsync(); // precisa do details.Id pro vínculo (se usar details)

            foreach (var eq in equipments)
            {
                var link = new RoomEquipment
                {
                    RoomDetailsId = details.Id,
                    EquipmentId = eq.Id
                };
                db.Set<RoomEquipment>().Add(link);
            }

            await db.SaveChangesAsync();
        }
    }

    private static List<Equipment> EnsureDefaultEquipments(ApplicationDbContext db)
    {
        var baseList = new List<(string Name, string? Description)>
        {
            ("Datashow", "Projetor"),
            ("HDMI", "Cabo HDMI"),
            ("Ar-condicionado", null),
            ("Quadro", "Quadro branco"),
            ("Extensão", "Filtro/Extensão de energia"),
            ("Som", "Caixa de som"),
        };

        var result = new List<Equipment>();

        foreach (var item in baseList)
        {
            var existing = db.Equipment.FirstOrDefault(e => e.Name == item.Name);
            if (existing is null)
            {
                existing = new Equipment { Name = item.Name, Description = item.Description };
                db.Equipment.Add(existing);
            }
            result.Add(existing);
        }

        return result;
    }

    private static class RoomList
    {
        public static List<SeedRoomItem> ReturnRoomList()
        {
            return new List<SeedRoomItem>
            {
                new SeedRoomItem { Name = "Sala 01", Capacity = 30, IsReserveable = true, RoomType = 1, IsActive = true },
                new SeedRoomItem { Name = "Sala 02", Capacity = 40, IsReserveable = true, RoomType = 1, IsActive = true },
                new SeedRoomItem { Name = "Auditório", Capacity = 120, IsReserveable = true, RoomType = 2, IsActive = true  },
            };
        }
    }

    private sealed class SeedRoomItem
    {
        public string Name { get; set; } = "";
        public int Capacity { get; set; }
        public bool IsReserveable { get; set; }
        public int RoomType { get; set; }
        public bool IsActive { get; set; }
    }
}

