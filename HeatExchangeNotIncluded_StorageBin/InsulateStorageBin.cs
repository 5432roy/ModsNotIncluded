using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using STRINGS;

namespace HeatExchangeNotIncluded_StorageBin
{
    public class InsulateStorageBin : KMonoBehaviour
    {
        [MyCmpGet]
        private Storage storage;

        [Serialize]
        public bool allowManualPumpingStationFetching;

        [Serialize]
        private float userMaxCapacity = float.PositiveInfinity;

        private Tag[] forbidden_tags;

        public string SliderTitleKey => "Maximum Capacity";

        public string SliderUnits => GameUtil.GetCurrentMassUnit();
        public float GetSliderMax(int index)
        {
            return 200000f;
        }

        public float GetSliderMin(int index)
        {
            return 0.0f;
        }

        public float GetSliderValue(int index)
        {
            return userMaxCapacity;
        }

        public string GetSliderTooltip(int index)
        {
            return "Maximum mass to store in this bin";//string.Format(Strings.Get(GetSliderTooltipKey(0)), userMaxCapacity);
        }

        public string GetSliderTooltipKey(int index)
        {
            return "";
        }
        public void SetSliderValue(float value, int index)
        {
            if (value != userMaxCapacity) //setslidervalue runs each time slider appears AND if changed - check if actually changed to avoid unncessary job interruptions
            {
                if (value > 100f)
                {
                    value = (float)Math.Round((decimal)value);
                    //will round off decimals above 100kg to avoid weird 5g bits when slider is moved instead of typed number
                }
                storage.capacityKg = value;
                userMaxCapacity = value; //set both local and Storage variable, local variable gets kept on save/load
                filteredStorage.FilterChanged();
            }
        }

        public float AmountStored => storage.MassStored();


        protected override void OnPrefabInit()
        {
            Initialize(use_logic_meter: false);
        }

        protected FilteredStorageInsulateStorageBin filteredStorage;

        public string choreTypeID = Db.Get().ChoreTypes.StorageFetch.Id;

        private static readonly EventSystem.IntraObjectHandler<InsulateStorageBin> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<InsulateStorageBin>(delegate (InsulateStorageBin component, object data)
        {
            component.OnCopySettings(data);
        });
        private static readonly EventSystem.IntraObjectHandler<InsulateStorageBin> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<InsulateStorageBin>(delegate (InsulateStorageBin component, object data)
        {
            component.OnRefreshUserMenu(data);
        });

        protected void Initialize(bool use_logic_meter)
        {
            //initialize comes first, then spawn
            base.OnPrefabInit();

            ChoreType fetch_chore_type = Db.Get().ChoreTypes.Get(choreTypeID);

            forbidden_tags = (allowManualPumpingStationFetching ? new Tag[0] : new Tag[1] { GameTags.LiquidSource });

            filteredStorage = new FilteredStorageInsulateStorageBin(this, forbidden_tags, null, use_logic_meter, fetch_chore_type);
            //replacing capacity_control slider in filteredstorage - leave it null and do the logic for it here
            //forbidden tags contains either nothing or pump stations. have to make own copy of filteredstorage just to keep this private field updated

            Subscribe(-905833192, OnCopySettingsDelegate);
            Subscribe(493375141, OnRefreshUserMenuDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            forbidden_tags = (allowManualPumpingStationFetching ? new Tag[0] : new Tag[1] { GameTags.LiquidSource });
            filteredStorage.SetForbiddenTags(forbidden_tags);
            filteredStorage.FilterChanged();

        }
        private void OnChangeAllowManualPumpingStationFetching()
        {
            allowManualPumpingStationFetching = !allowManualPumpingStationFetching;

            forbidden_tags = (allowManualPumpingStationFetching ? new Tag[0] : new Tag[1] { GameTags.LiquidSource });
            filteredStorage.SetForbiddenTags(forbidden_tags);
            filteredStorage.FilterChanged();

        }

        protected override void OnCleanUp()
        {
            filteredStorage.CleanUp();
        }


        private void OnCopySettings(object data)
        {
            GameObject gameObject = (GameObject)data;
            if (!(gameObject == null))
            {
                InsulateStorageBin component = gameObject.GetComponent<InsulateStorageBin>();
                if (!(component == null))
                {
                    //this is copying settings TO the local variables from clipboard component
                    userMaxCapacity = component.userMaxCapacity;
                    storage.capacityKg = userMaxCapacity;
                    allowManualPumpingStationFetching = component.allowManualPumpingStationFetching;
                    forbidden_tags = (allowManualPumpingStationFetching ? new Tag[0] : new Tag[1] { GameTags.LiquidSource });
                    filteredStorage.SetForbiddenTags(forbidden_tags);
                    filteredStorage.FilterChanged();
                }
            }
        }

        private void OnRefreshUserMenu(object data)
        {
            KIconButtonMenu.ButtonInfo button2 = (allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, OnChangeAllowManualPumpingStationFetching, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, OnChangeAllowManualPumpingStationFetching, Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP));
            Game.Instance.userMenu.AddButton(base.gameObject, button2, 0.4f);
        }

    }

    public class DeconstructableInsulateStorageBin : Workable
    {

        //modified deconstructable to replace default behavior, this one will deconstruct instantly when given decon order
        //however it won't drop any resources from the building itself, important because it's made of vacuum and this gives an error
        //also drops gas resource in canister form

        private static readonly EventSystem.IntraObjectHandler<DeconstructableInsulateStorageBin> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DeconstructableInsulateStorageBin>(delegate (DeconstructableInsulateStorageBin component, object data)
        {
            component.OnRefreshUserMenu(data);
        });
        private static readonly EventSystem.IntraObjectHandler<DeconstructableInsulateStorageBin> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<DeconstructableInsulateStorageBin>(delegate (DeconstructableInsulateStorageBin component, object data)
        {
            component.OnDeconstruct();
        });
        private CellOffset[] placementOffsets
        {
            get
            {
                Building component = GetComponent<Building>();
                if (component != null)
                {
                    return component.Def.PlacementOffsets;
                }

                Debug.Assert(condition: false, "There's some error with Insulate Storage mod that the developer doesn't understand", this);
                return null;

            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(493375141, OnRefreshUserMenuDelegate);
            Subscribe(-111137758, OnRefreshUserMenuDelegate);
            Subscribe(-790448070, OnDeconstructDelegate);

            CellOffset[][] table = OffsetGroups.InvertedStandardTable;
            CellOffset[] filter = null;
            CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(placementOffsets, table, filter);
            SetOffsetTable(offsetTable);
            //I really don't know what this celloffset stuff is about, too afraid to delete
            //from original deconstructable class


        }
        protected override void OnSpawn()
        {
            base.OnSpawn();

        }
        public void OnDeconstruct()
        {

            Storage storage = base.GetComponent<Storage>();
            InsulateStorageBin InsulateStorageBin = base.GetComponent<InsulateStorageBin>();

            storage.DropAll(vent_gas: false, dump_liquid: false, do_disease_transfer: true);

            base.gameObject.DeleteObject(); //goodbye
        }


        private void OnRefreshUserMenu(object data)
        {
            if (!this.HasTag(GameTags.Stored))
            {
                //add deconstruct button
                KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo("action_deconstruct", InsulateStorageBinConfig.DeconstructButtonText, OnDeconstruct, Action.NumActions, null, null, null, InsulateStorageBinConfig.DeconstructButtonTooltip);//: new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.DECONSTRUCT.NAME_OFF, OnDeconstruct, Action.NumActions, null, null, null, UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP_OFF));
                Game.Instance.userMenu.AddButton(base.gameObject, button, 0f);
            }
        }
    }


    public class FilteredStorageInsulateStorageBin
    {
        //this class is basically a copy of filteredstorage with just a few changes necessary to make this mod works
        //for example, handling of forbidden tags for the auto bottler

        public static readonly HashedString FULL_PORT_ID = "FULL";

        private KMonoBehaviour root;

        private FetchList2 fetchList;

        private IUserControlledCapacity capacityControl;

        private TreeFilterable filterable;

        private Storage storage;

        private MeterController meter;

        private MeterController logicMeter;

        private Tag[] forbiddenTags;

        private bool hasMeter = true;

        private bool useLogicMeter;

        private ChoreType choreType;

        public void SetHasMeter(bool has_meter)
        {
            hasMeter = has_meter;
        }

        public FilteredStorageInsulateStorageBin(KMonoBehaviour root, Tag[] forbidden_tags, IUserControlledCapacity capacity_control, bool use_logic_meter, ChoreType fetch_chore_type)
        {
            this.root = root;
            forbiddenTags = forbidden_tags;
            capacityControl = capacity_control;
            useLogicMeter = use_logic_meter;
            choreType = fetch_chore_type;
            root.Subscribe(-1697596308, OnStorageChanged);
            root.Subscribe(-543130682, OnUserSettingsChanged);
            filterable = root.FindOrAdd<TreeFilterable>();
            TreeFilterable treeFilterable = filterable;
            treeFilterable.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<HashSet<Tag>>(OnFilterChanged));
            storage = root.GetComponent<Storage>();
            storage.Subscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
            storage.Subscribe(-1852328367, OnFunctionalChanged);
        }

        private void OnOnlyFetchMarkedItemsSettingChanged(object data)
        {
            OnFilterChanged(filterable.GetTags());
        }

        private void CreateMeter()
        {
            if (hasMeter)
            {
                meter = new MeterController(root.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_frame", "meter_level");
            }
        }

        private void CreateLogicMeter()
        {
            if (hasMeter)
            {
                logicMeter = new MeterController(root.GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
            }
        }

        public void CleanUp()
        {
            if (filterable != null)
            {
                TreeFilterable treeFilterable = filterable;
                treeFilterable.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Remove(treeFilterable.OnFilterChanged, new Action<HashSet<Tag>>(OnFilterChanged));
            }
            if (fetchList != null)
            {
                fetchList.Cancel("Parent destroyed");
            }
        }

        public void FilterChanged()
        {
            if (hasMeter)
            {
                if (meter == null)
                {
                    CreateMeter();
                }
                if (logicMeter == null && useLogicMeter)
                {
                    CreateLogicMeter();
                }
            }
            OnFilterChanged(filterable.GetTags());
            UpdateMeter();
        }

        private void OnUserSettingsChanged(object data)
        {
            OnFilterChanged(filterable.GetTags());
            UpdateMeter();
        }

        private void OnStorageChanged(object data)
        {
            if (fetchList == null)
            {
                OnFilterChanged(filterable.GetTags());
            }
            UpdateMeter();
        }

        private void OnFunctionalChanged(object data)
        {
            OnFilterChanged(filterable.GetTags());
        }

        private void UpdateMeter()
        {
            float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
            float positionPercent = Mathf.Clamp01(GetAmountStored() / maxCapacityMinusStorageMargin);
            if (meter != null)
            {
                meter.SetPositionPercent(positionPercent);
            }
        }

        public bool IsFull()
        {
            float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
            float num = Mathf.Clamp01(GetAmountStored() / maxCapacityMinusStorageMargin);
            if (meter != null)
            {
                meter.SetPositionPercent(num);
            }
            if (!(num >= 1f))
            {
                return false;
            }
            return true;
        }

        private void OnFetchComplete()
        {
            OnFilterChanged(filterable.GetTags());
        }

        private float GetMaxCapacity()
        {
            float num = storage.capacityKg;
            if (capacityControl != null)
            {
                num = Mathf.Min(num, capacityControl.UserMaxCapacity);
            }
            return num;
        }

        private float GetMaxCapacityMinusStorageMargin()
        {
            return GetMaxCapacity() - storage.storageFullMargin;
        }

        private float GetAmountStored()
        {
            float result = storage.MassStored();
            if (capacityControl != null)
            {
                result = capacityControl.AmountStored;
            }
            return result;
        }

        private bool IsFunctional()
        {
            Operational component = storage.GetComponent<Operational>();
            if (!(component == null))
            {
                return component.IsFunctional;
            }
            return true;
        }

        public void SetForbiddenTags(Tag[] forbidden_tags)
        {
            forbiddenTags = forbidden_tags; //wouldn't need this whole class except for that
                                            //and actually, after the update 10/4 which added new public methods to modify forbidden tags, may well be entirely unnecessary
                                            //but... if it ain't broke, I'm not fixing it
        }

        private void OnFilterChanged(HashSet<Tag> tags)
        {
            bool flag = tags != null && tags.Count != 0;
            if (fetchList != null)
            {
                fetchList.Cancel("");
                fetchList = null;
            }
            float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
            float amountStored = GetAmountStored();
            float num = Mathf.Max(0f, maxCapacityMinusStorageMargin - amountStored);
            if (num > 0f && flag && IsFunctional())
            {
                num = Mathf.Max(0f, GetMaxCapacity() - amountStored);
                fetchList = new FetchList2(storage, choreType);
                fetchList.ShowStatusItem = false;
                fetchList.Add(tags, forbiddenTags, num, Operational.State.Functional);
                fetchList.Submit(OnFetchComplete, check_storage_contents: false);
            }
        }

        public void SetLogicMeter(bool on)
        {
            if (logicMeter != null)
            {
                logicMeter.SetPositionPercent(on ? 1f : 0f);
            }
        }
    }
}