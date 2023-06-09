﻿using X.PagedList;

namespace PBL3_Server.Services.DisposedAssetService
{
    public class DisposedAssetService : IDisposedAssetService
    {
        private static List<DisposedAsset> DisposedAssets = new List<DisposedAsset>();


        private readonly DataContext _context;

        public DisposedAssetService(DataContext context)
        {
            this._context = context;
        }

        public async Task<List<DisposedAsset>> AddDisposedAsset(DisposedAsset asset)
        {
            _context.DisposedAssets.Add(asset);
            await _context.SaveChangesAsync();
            return DisposedAssets;
        }

        public async Task<List<DisposedAsset>?> DeleteDisposedAsset(string id)
        {
            var asset = await _context.DisposedAssets.FindAsync(id);
            if (asset is null)
                return null;
            _context.Remove(asset);
            await _context.SaveChangesAsync();
            return DisposedAssets;
        }

        public async Task<List<DisposedAsset>> GetAllDisposedAssets()
        {
            var assets = await _context.DisposedAssets.ToListAsync();
            return assets;
        }

        public async Task<DisposedAsset?> GetSingleDisposedAsset(string id)
        {
            var asset = await _context.DisposedAssets.FindAsync(id);
            if (asset is null)
                return null;
            return asset;
        }

        public async Task<List<DisposedAsset>?> UpdateDisposedAsset(string id, DisposedAsset request)
        {
            var asset = await _context.DisposedAssets.FindAsync(id);
            if (asset is null)
                return null;

            asset.AssetID = request.AssetID;
            asset.DeviceID = request.DeviceID;
            asset.RoomID = request.RoomID;
            asset.AssetName = request.AssetName;
            asset.YearOfUse = request.YearOfUse;
            asset.TechnicalSpecification = request.TechnicalSpecification;
            asset.Quantity = request.Quantity;
            asset.Cost = request.Cost;
            asset.PercentageCL = request.PercentageCL;
            asset.DateDisposed = request.DateDisposed;
            asset.Notes = request.Notes;

            await _context.SaveChangesAsync();

            return DisposedAssets;
        }


        public async Task<List<DisposedAsset>> CancelDisposeAsset(string id)
        {
            var disposedasset = await _context.DisposedAssets.FindAsync(id);
            if (disposedasset is null)
                return null;

            var asset = new Asset
            {
                AssetID = disposedasset.AssetID,
                DeviceID = disposedasset.DeviceID,
                RoomID = disposedasset.RoomID,
                AssetName = disposedasset.AssetName,
                YearOfUse = disposedasset.YearOfUse,
                TechnicalSpecification = disposedasset.TechnicalSpecification,
                Status = disposedasset.Status,
                Quantity = disposedasset.Quantity,
                Cost = disposedasset.Cost,
                PercentageCL = disposedasset.PercentageCL,
                Notes = disposedasset.Notes
            };

            _context.Remove(disposedasset);
            _context.Add(asset);
            await _context.SaveChangesAsync();
            return DisposedAssets;
        }

        public async Task<int> StatisticDisposeAsset(string organization_id, int year_of_use, int year_dispose)
        {
            IQueryable<DisposedAsset> assets = _context.DisposedAssets;

            if (!string.IsNullOrEmpty(organization_id))
            {
                IQueryable<Room> rooms = _context.Rooms
                    .Where(r => r.organizationID.ToLower() == organization_id.ToLower());

                assets = assets.Where(a => rooms.Any(r => r.RoomID == a.RoomID));
            }

            if (year_of_use > 0)
            {
                assets = assets.Where(a => a.YearOfUse == year_of_use);
            }

            if (year_dispose > 0)
            {
                assets = assets.Where(a => a.DateDisposed.Year == year_dispose);
            }

            int count = await assets.CountAsync();
            return count;
        }

    }
}
