using Aki.Custom.CustomAI;
using Aki.Reflection.Patching;
using EFT;
using EFT.InventoryLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyTweaks
{
    internal class Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            Type pmcFoundInRaidEquipment = typeof(PmcFoundInRaidEquipment);
            return pmcFoundInRaidEquipment.GetMethod("ConfigurePMCFindInRaidStatus");
        }

        [PatchPrefix]
        private static bool Prefix(object[] __args)
        {
            BotOwner botOwner = __args[0] as BotOwner;
            MakeEquipmentFiR(botOwner);

            IReadOnlyList<Slot> containerGear = botOwner.Profile.Inventory.Equipment.ContainerSlots;
            foreach (var container in containerGear)
            {
                var firstItem = container.Items.FirstOrDefault();
                foreach (var item in container.ContainedItem.GetAllItems())
                {
                    item.SpawnedInSession = true;
                }
            }

            // Set dogtag as FiR
            var dogtag = botOwner.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[] { EquipmentSlot.Dogtag }).FirstOrDefault();
            if (dogtag != null)
            {
                dogtag.SpawnedInSession = true;
            }

            return false;
        }

        // Copy of MakeEquipmentNotFiR but flipped
        private static void MakeEquipmentFiR(BotOwner botOwner)
        {
            var additionalItems = botOwner.Profile.Inventory.GetItemsInSlots(new EquipmentSlot[]
            {   EquipmentSlot.Backpack,
                EquipmentSlot.FirstPrimaryWeapon,
                EquipmentSlot.SecondPrimaryWeapon,
                EquipmentSlot.TacticalVest,
                EquipmentSlot.ArmorVest,
                EquipmentSlot.Scabbard,
                EquipmentSlot.Eyewear,
                EquipmentSlot.Headwear,
                EquipmentSlot.Earpiece,
                EquipmentSlot.ArmBand,
                EquipmentSlot.FaceCover,
                EquipmentSlot.Holster,
                EquipmentSlot.SecuredContainer
            });

            foreach (var item in additionalItems)
            {
                // Some items are null, probably because bot doesnt have that particular slot on them
                if (item == null)
                {
                    continue;
                }

                //Logger.LogError($"flagging item FiR: {item.Id} {item.Name} _parent: {item.Template._parent}");
                item.SpawnedInSession = true;
            }
        }
    }
}
