using System.Threading.Tasks;
using Zenject;

namespace Managers
{
    public class BuildingManager : IBuildingManager
    {
        [Inject] private DiContainer _diContainer;
        private IBuilding _previousBuilding;

        public async Task<IBuilding> CreateBuilding(BuildingID id)
        {
            _previousBuilding?.loader.UnloadAsset();
            var loader = new AssetsLoader();
            var building = await loader.InstantiateAssetWithDI<IBuilding>(id.ToString(), _diContainer);
            building.loader = loader;
            building.transform.position = Constants.BUILDING_POSITION;
            building.Init(new object());
            _previousBuilding = building;
            return building;
        }
    }
}