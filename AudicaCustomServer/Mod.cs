using HarmonyLib;
using MelonLoader;

namespace AudicaCustomServer
{
    public class Mod : MelonMod
    {
        private static MelonLogger.Instance _logger;
        
        public override void OnApplicationStart()
        {
            _logger = LoggerInstance;
            var harmony = HarmonyInstance;
            var methodCreateRequest = typeof(NetRequestManager).GetMethod("CreateRequest", new[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
            var methodDeobfuscateAddress = typeof(NetRequestManager).GetMethod("DeobfuscateAddress", new[] { typeof(string) });

            // Temp is only here because we need to pass a reference to a variable
            var tmp = "";

            harmony.Patch(methodCreateRequest, new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => PreCreateRequest("", ref tmp, "", "", ""))));
            harmony.Patch(methodDeobfuscateAddress,
                postfix: new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => PostDeobfuscateAddress("", ref tmp))));
        }

        static void PreCreateRequest(string address,
            ref string data,
            string authToken,
            string authTokenPrefix,
            string requestType)
        {
            _logger.Msg("NetRequestManager#CreateRequest address=" + address + ", data=" + data + ", authToken=" + authToken + ", authTokenPrefix=" + authTokenPrefix + ", requestType=" + requestType);
        }

        static void PostDeobfuscateAddress(string address, ref string __result)
        {
            // NOTE: DeobfuscateAddress is also called to deobfuscate api endpoint strings like /auth/steam/token/create/
            if (__result == "https://audica-prod-api.hmxwebservices.com")
            {
                __result = "http://localhost";
            }
            _logger.Msg("NetRequestManager#DeobfuscateAddress " + address + " -> " + __result);
        }
    }
}