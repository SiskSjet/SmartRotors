using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using VRage;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace RealSun
{
    internal class RealStarsApi
    {
        private bool _apiInit;

        private Action<Vector3D, float, Vector3I, float, float, float, float, float> _spawnStar;
        private Func<MyPlanet, MyTuple<bool, float, Vector3I, float, float, float>> _getStarInfo;
        private Func<MyPlanet, MyTuple<bool, float, float>> _getStarGravity;
        private Func<MyPlanet, float, Vector3I, float, float, float, float, float, bool> _setStarInfo;
        private Action<string, Vector3D, float, int> _spawnPlanetNoCycle;
        private Action<string, Vector3D, float, int, float, bool, float> _spawnPlanetWithCycle;
        private Func<MyPlanet, MyTuple<bool, bool, float, bool>> _getPlanetInfo;
        private Func<MyPlanet, bool, float, bool, bool> _setPlanetInfo;
        private Func<Vector3D, MyTuple<Vector3D, float>> _getSunInfoAtPosition;
        private Func<Vector3D, MyTuple<MyPlanet, MyPlanet, Vector3D, float, float>> _getSunInfoWithPlanetAtPosition;

        private const long Channel = 635917436927;

        public bool IsReady { get; private set; }
        public bool Compromised { get; private set; }
        private void HandleMessage(object o)
        {
            if (_apiInit) return;
            var dict = o as IReadOnlyDictionary<string, Delegate>;
            var message = o as string;

            if (message != null && message == "Compromised")
                Compromised = true;

            if (dict == null || dict is ImmutableDictionary<string, Delegate>)
                return;

            var builder = ImmutableDictionary.CreateBuilder<string, Delegate>();
            foreach (var pair in dict)
                builder.Add(pair.Key, pair.Value);

            MyAPIGateway.Utilities.SendModMessage(Channel, builder.ToImmutable());

            ApiLoad(dict);
            IsReady = true;
        }

        private bool _isRegistered;

        public bool Load()
        {
            if (!_isRegistered)
            {
                _isRegistered = true;
                MyAPIGateway.Utilities.RegisterMessageHandler(Channel, HandleMessage);
            }
            if (!IsReady)
                MyAPIGateway.Utilities.SendModMessage(Channel, "ApiEndpointRequest");
            return IsReady;
        }

        public void Unload()
        {
            if (_isRegistered)
            {
                _isRegistered = false;
                MyAPIGateway.Utilities.UnregisterMessageHandler(Channel, HandleMessage);
            }
            IsReady = false;
        }

        public void ApiLoad(IReadOnlyDictionary<string, Delegate> delegates)
        {
            _apiInit = true;

            _spawnStar = (Action<Vector3D, float, Vector3I, float, float, float, float, float>)delegates["SpawnStar"];
            _getStarInfo = (Func<MyPlanet, MyTuple<bool, float, Vector3I, float, float, float>>)delegates["GetStarInfo"];
            _getStarGravity = (Func<MyPlanet, MyTuple<bool, float, float>>)delegates["GetStarGravity"];
            _setStarInfo = (Func<MyPlanet, float, Vector3I, float, float, float, float, float, bool>)delegates["SetStarInfo"];
            _spawnPlanetNoCycle = (Action<string, Vector3D, float, int>)delegates["SpawnPlanetNoCycle"];
            _spawnPlanetWithCycle = (Action<string, Vector3D, float, int, float, bool, float>)delegates["SpawnPlanetWithCycle"];
            _getPlanetInfo = (Func<MyPlanet, MyTuple<bool, bool, float, bool>>)delegates["GetPlanetInfo"];
            _setPlanetInfo = (Func<MyPlanet, bool, float, bool, bool>)delegates["SetPlanetInfo"];
            _getSunInfoAtPosition = (Func<Vector3D, MyTuple<Vector3D, float>>)delegates["GetSunInfoAtPosition"];
            _getSunInfoWithPlanetAtPosition = (Func<Vector3D, MyTuple<MyPlanet, MyPlanet, Vector3D, float, float>>)delegates["GetSunInfoWithPlanetAtPosition"];
        }


        public void SpawnStar(Vector3D position, float radius, Vector3I color, float effectBrightness, float lightBrightness, float damageRadius, float gravityStrength, float gravityFalloff) => _spawnStar?.Invoke(position, radius, color, effectBrightness, lightBrightness, damageRadius, gravityStrength, gravityFalloff);
        public MyTuple<bool, float, Vector3I, float, float, float> GetStarInfo(MyPlanet planet) => _getStarInfo?.Invoke(planet) ?? new MyTuple<bool, float, Vector3I, float, float, float>();
        public MyTuple<bool, float, float> GetStarGravity(MyPlanet planet) => _getStarGravity?.Invoke(planet) ?? new MyTuple<bool, float, float>();
        public bool SetStarInfo(MyPlanet planet, float radius, Vector3I color, float effectBrightness, float lightBrightness, float damageRadius, float gravityStrength, float gravityFalloff) => _setStarInfo?.Invoke(planet, radius, color, effectBrightness, lightBrightness, damageRadius, gravityStrength, gravityFalloff) ?? false;
        public void SpawnPlanetNoCycle(string planetName, Vector3D position, float radius, int seed) => _spawnPlanetNoCycle?.Invoke(planetName, position, radius, seed);
        public void SpawnPlanetWithCycle(string planetName, Vector3D position, float radius, int seed, float daySeconds, bool isCounterClockwise, float extraZoneRadiusKm) => _spawnPlanetWithCycle?.Invoke(planetName, position, radius, seed, daySeconds, isCounterClockwise, extraZoneRadiusKm);
        public MyTuple<bool, bool, float, bool> GetPlanetInfo(MyPlanet planet) => _getPlanetInfo?.Invoke(planet) ?? new MyTuple<bool, bool, float, bool>();
        public bool SetPlanetInfo(MyPlanet planet, bool dayCycleEnabled, float dayLengthSeconds, bool spinCounterClockwise) => _setPlanetInfo?.Invoke(planet, dayCycleEnabled, dayLengthSeconds, spinCounterClockwise) ?? false;
        public MyTuple<Vector3D, float> GetSunInfoAtPosition(Vector3D position) => _getSunInfoAtPosition?.Invoke(position) ?? new MyTuple<Vector3D, float>();
        public MyTuple<MyPlanet, MyPlanet, Vector3D, float, float> GetSunInfoWithPlanetAtPosition(Vector3D position) => _getSunInfoWithPlanetAtPosition?.Invoke(position) ?? new MyTuple<MyPlanet, MyPlanet, Vector3D, float, float>();
    }
}
