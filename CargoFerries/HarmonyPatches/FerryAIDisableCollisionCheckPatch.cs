using CargoFerries.AI;
using CargoFerries.Utils;

namespace CargoFerries.HarmonyPatches
{
    public class FerryAIDisableCollisionCheckPatch
    {
        private static bool deployed;

        public static void Apply()
        {
            if (deployed)
            {
                return;
            }

            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(FerryAI), "DisableCollisionCheck"),
                new PatchUtil.MethodDefinition(typeof(FerryAIDisableCollisionCheckPatch), nameof(Prefix)));

            deployed = true;
        }

        public static void Undo()
        {
            if (!deployed)
            {
                return;
            }

            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(FerryAI), "DisableCollisionCheck"));

            deployed = false;
        }

        public static bool Prefix(ushort vehicleID, ref bool __result)
        {
            var vehicleAi = VehicleManager.instance.m_vehicles.m_buffer[vehicleID].Info?.m_vehicleAI;
            if (vehicleAi is FakeFerryAI)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}