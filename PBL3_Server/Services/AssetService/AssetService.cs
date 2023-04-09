﻿using Microsoft.EntityFrameworkCore;
using PBL3_Server.Services.RoomService;

namespace PBL3_Server.Services.AssetService
{
    public class AssetService : IAssetService
    {
        private static List<Asset> Assets = new List<Asset>();
            

        private readonly DataContext _context;

        public AssetService(DataContext context) 
        {
            _context = context;
        }

        public async Task<List<Asset>> AddAsset(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
            return Assets;
        }

        public async Task<List<Asset>> DeleteAsset(string id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset is null)
                return null;
            _context.Remove(asset);
            await _context.SaveChangesAsync();
            return Assets;
        }

        public async Task<List<Asset>> GetAllAssets()
        {
            var assets = await _context.Assets.ToListAsync();
            return assets;
        }

        public async Task<Asset> GetSingleAsset(string id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset is null)
                return null;
            return asset;
        }

        public async Task<List<Asset>> UpdateAsset(string id, Asset request)
        {
            var asset = await _context.Assets.FindAsync(id);
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
            asset.Status = request.Status;
            asset.Notes = request.Notes;

            await _context.SaveChangesAsync();

            return Assets;
        }
        public async Task<List<Asset>> DisposedAsset(string id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset is null)
                return null;   
            var disposedAsset = new DisposedAsset
            {
                AssetID = asset.AssetID,
                DeviceID = asset.DeviceID,
                RoomID = asset.RoomID,
                AssetName = asset.AssetName,
                YearOfUse = asset.YearOfUse,
                TechnicalSpecification = asset.TechnicalSpecification,
                Quantity = asset.Quantity,
                Cost = asset.Cost,
                DateDisposed = DateTime.Now,
                Status = asset.Status,
                Notes = asset.Notes
            };
            
            _context.Remove(asset);
            _context.Add(disposedAsset);
            await _context.SaveChangesAsync();
            return Assets;
        }
    }
}
