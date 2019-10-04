using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace DwarfCorp
{
    public class CraftItem
    {
        public enum CraftType
        {
            Object,
            Resource
        }

        public enum CraftPrereq
        {
            OnGround,
            NearWall
        }

        public enum CraftActBehaviors
        {
            Normal,
            Trinket,
            Meal,
            Ale,
            Bread,
            GemTrinket,
            Object
        }

        public string Name = "";
        public string EntityName = "";
        public string ObjectName = "";

        public String DisplayName = null;
        public String ShortDisplayName = null;
        public String PluralDisplayName = null;

        public List<ResourceTagAmount> RequiredResources = new List<ResourceTagAmount>();
        public Gui.TileReference Icon = null;
        public float BaseCraftTime = 0.0f;
        public string Description = "";
        public CraftType Type = CraftType.Object;
        public List<CraftPrereq> Prerequisites = new List<CraftPrereq>();
        public int CraftedResultsCount = 1;
        public String ResourceCreated = "";
        public string CraftLocation = "Anvil";
        public string Verb = null;
        public string PastTeseVerb = null;
        public string CurrentVerb = null;
        public bool AllowHeterogenous = false;
        public Vector3 SpawnOffset = new Vector3(0.0f, 0.5f, 0.0f);
        public bool AddToOwnedPool = false;
        public bool Deconstructable = true;
        public CraftActBehaviors CraftActBehavior = CraftActBehaviors.Normal;
        public bool AllowRotation = false;
        public string Category = "";
        public bool IsMagical = false;
        public string Tutorial = "";
        public bool AllowUserCrafting = true;
        public TaskCategory CraftTaskCategory = TaskCategory.CraftItem;
        public string CraftNoise = "Craft";
        public DwarfBux MoneyValue = 20.0m;

        public bool Disable = false;

        public void InitializeStrings()
        {
            DisplayName = Library.TransformDataString(DisplayName, Name);
            PluralDisplayName = Library.TransformDataString(PluralDisplayName, DisplayName + "s"); // Default to appending an s if the plural name is not specified.
            ShortDisplayName = Library.TransformDataString(ShortDisplayName, DisplayName);
            Verb = Library.TransformDataString(Verb, Library.GetString("build"));
            PastTeseVerb = Library.TransformDataString(PastTeseVerb, Library.GetString("built"));
            CurrentVerb = Library.TransformDataString(CurrentVerb, Library.GetString("building"));
            Description = Library.TransformDataString(Description, Description);
        }

        private IEnumerable<ResourceTypeAmount> MergeResources(IEnumerable<ResourceTypeAmount> resources)
        {
            Dictionary<String, int> counts = new Dictionary<String, int>();
            foreach(var resource in resources)
            {
                if(!counts.ContainsKey(resource.Type))
                {
                    counts.Add(resource.Type, 0);
                }
                counts[resource.Type] += resource.Count;
            }

            foreach(var count in counts)
            {
                yield return new ResourceTypeAmount(count.Key, count.Value);
            }
        }

        public ResourceType ToResource(WorldManager world, string prefix = "")
        {
            var objectName = String.IsNullOrEmpty(ObjectName) ? Name : ObjectName;
            string resourceName = prefix + objectName;// + " (" + TextGenerator.GetListString(MergeResources(selectedResources).Select(r => (string)r.Type)) + ")";

            if (Library.GetResourceType(resourceName).HasValue(out var existing))
                return existing;

            var sheet = world.UserInterface.Gui.RenderData.SourceSheets[Icon.Sheet];

            var tex = AssetManager.GetContentTexture(sheet.Texture);
            var numTilesX = tex.Width / sheet.TileWidth;
            var numTilesY = tex.Height / sheet.TileHeight;
            var point = new Point(Icon.Tile % numTilesX, Icon.Tile / numTilesX);
            var toReturn = Library.CreateResourceType();
            toReturn.Name = resourceName;
            toReturn.Tags = new List<String>()
                    {
                        "CraftItem",
                        "Craft"
                    };
            toReturn.MoneyValue = MoneyValue;
            toReturn.CraftInfo = new ResourceType.CraftItemInfo
            {
                CraftItemType = objectName
            };
            toReturn.ShortName = Name;
            toReturn.Description = Description;
            toReturn.GuiLayers = new List<Gui.TileReference>() { Icon };
            toReturn.CompositeLayers = new List<ResourceType.CompositeLayer>() { new ResourceType.CompositeLayer() { Asset = sheet.Texture, Frame = point, FrameSize = new Point(sheet.TileWidth, sheet.TileHeight) } };
            toReturn.Tint = Color.White;
            Library.AddResourceType(toReturn);

            return toReturn;
        }

        public CraftItem ObjectAsCraftableResource()
        {
            //return this; 
            if (Type == CraftType.Resource)
                return this;


            string resourceName = Name + "...";
            if (Library.GetCraftable(resourceName).HasValue(out var _r))
                return _r;

            var r = this.MemberwiseClone() as CraftItem;
            r.Name = resourceName;
            r.Type = CraftType.Resource;
            r.CraftActBehavior = CraftActBehaviors.Object;
            r.ResourceCreated = Name;
            r.CraftLocation = /*String.IsNullOrEmpty(CraftLocation) ? "Anvil" :*/ CraftLocation;
            r.ObjectName = Name;
            r.AllowUserCrafting = false;
            r.Category = this.Category;
            r.CraftTaskCategory = this.CraftTaskCategory;
            r.CraftNoise = this.CraftNoise;
            Library.AddCraftable(r);

            // Todo: Obsolete when new building system is in place.

            return r;
        }
    }
}