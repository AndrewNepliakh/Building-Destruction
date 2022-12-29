using System.Threading.Tasks;

namespace Managers
{
    public interface IBuildingManager
    {
        public Task<IBuilding> CreateBuilding(BuildingID id);
    }
}