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

namespace RealSun {

    public class RealStarsApi {
        private const long Channel = 635917436927;
        private bool _apiInit;

        private Func<Vector3D> _getCurrentRealSunDirection;
        private Func<float> _getCurrentRealSunDist;
        private Func<float> _getCurrentRealSunRad;
        private Func<MyPlanet, MyTuple<bool, bool, float, bool>> _getPlanetInfo;
        private Func<MyTuple<float, float>> _getSolarBlockMinMax;
        private Func<MyPlanet, MyTuple<bool, float, float>> _getStarGravity;
        private Func<MyPlanet, MyTuple<bool, float, Vector3I, float, float, float>> _getStarInfo;
        private Func<Vector3D, MyTuple<Vector3D, float, float>> _getSunInfoAtPosition;
        private Func<Vector3D, MyTuple<MyPlanet, MyPlanet>> _getSunInfoNearbyPlanets;
        private Func<Vector3D, MyPlanet, MyPlanet, MyTuple<Vector3D, float, float, float, float>> _getSunInfoWithPlanetAtPosition;
        private bool _isRegistered;
        private Func<MyPlanet, bool, float, bool, bool> _setPlanetInfo;
        private Func<MyPlanet, float, Vector3I, float, float, float, float, float, bool> _setStarInfo;
        private Action<string, Vector3D, float, int> _spawnPlanetNoCycle;
        private Action<string, Vector3D, float, int, float, bool, float> _spawnPlanetWithCycle;
        private Action<Vector3D, float, Vector3I, float, float, float, float, float> _spawnStar;
        public bool Compromised { get; private set; }
        public bool IsReady { get; private set; }

        public void ApiLoad(IReadOnlyDictionary<string, Delegate> delegates) {
            _apiInit = true;

            _spawnStar = (Action<Vector3D, float, Vector3I, float, float, float, float, float>)delegates["SpawnStar"];
            _getStarInfo = (Func<MyPlanet, MyTuple<bool, float, Vector3I, float, float, float>>)delegates["GetStarInfo"];
            _getStarGravity = (Func<MyPlanet, MyTuple<bool, float, float>>)delegates["GetStarGravity"];
            _setStarInfo = (Func<MyPlanet, float, Vector3I, float, float, float, float, float, bool>)delegates["SetStarInfo"];
            _spawnPlanetNoCycle = (Action<string, Vector3D, float, int>)delegates["SpawnPlanetNoCycle"];
            _spawnPlanetWithCycle = (Action<string, Vector3D, float, int, float, bool, float>)delegates["SpawnPlanetWithCycle"];
            _getPlanetInfo = (Func<MyPlanet, MyTuple<bool, bool, float, bool>>)delegates["GetPlanetInfo"];
            _setPlanetInfo = (Func<MyPlanet, bool, float, bool, bool>)delegates["SetPlanetInfo"];
            _getSunInfoAtPosition = (Func<Vector3D, MyTuple<Vector3D, float, float>>)delegates["GetSunInfoAtPosition"];
            _getSunInfoNearbyPlanets = (Func<Vector3D, MyTuple<MyPlanet, MyPlanet>>)delegates["GetSunInfoNearbyPlanets"];
            _getSunInfoWithPlanetAtPosition = (Func<Vector3D, MyPlanet, MyPlanet, MyTuple<Vector3D, float, float, float, float>>)delegates["GetSunInfoWithPlanetAtPosition"];
            _getSolarBlockMinMax = (Func<MyTuple<float, float>>)delegates["GetSolarBlockMinMax"];
            _getCurrentRealSunDirection = (Func<Vector3D>)delegates["GetCurrentRealSunDirection"];
            _getCurrentRealSunDist = (Func<float>)delegates["GetCurrentRealSunDist"];
            _getCurrentRealSunRad = (Func<float>)delegates["GetCurrentRealSunRad"];
        }

        public Vector3D GetCurrentRealSunDirection() => _getCurrentRealSunDirection?.Invoke() ?? Vector3D.Zero;

        public float GetCurrentRealSunDist() => _getCurrentRealSunDist?.Invoke() ?? float.MaxValue;

        public float GetCurrentRealSunRad() => _getCurrentRealSunRad?.Invoke() ?? 0.045f;

        public MyTuple<bool, bool, float, bool> GetPlanetInfo(MyPlanet planet) => _getPlanetInfo?.Invoke(planet) ?? new MyTuple<bool, bool, float, bool>();

        public MyTuple<float, float> GetSolarBlockMinMax() => _getSolarBlockMinMax?.Invoke() ?? new MyTuple<float, float>(1f, 1f);

        public MyTuple<bool, float, float> GetStarGravity(MyPlanet planet) => _getStarGravity?.Invoke(planet) ?? new MyTuple<bool, float, float>();

        public MyTuple<bool, float, Vector3I, float, float, float> GetStarInfo(MyPlanet planet) => _getStarInfo?.Invoke(planet) ?? new MyTuple<bool, float, Vector3I, float, float, float>();

        public MyTuple<Vector3D, float, float> GetSunInfoAtPosition(Vector3D position) => _getSunInfoAtPosition?.Invoke(position) ?? new MyTuple<Vector3D, float, float>();

        public MyTuple<MyPlanet, MyPlanet> GetSunInfoNearbyPlanets(Vector3D position) => _getSunInfoNearbyPlanets?.Invoke(position) ?? new MyTuple<MyPlanet, MyPlanet>();

        public MyTuple<Vector3D, float, float, float, float> GetSunInfoWithPlanetAtPosition(Vector3D position, MyPlanet planetZone, MyPlanet planetNearest) => _getSunInfoWithPlanetAtPosition?.Invoke(position, planetZone, planetNearest) ?? new MyTuple<Vector3D, float, float, float, float>();

        public bool Load() {
            if (!_isRegistered) {
                _isRegistered = true;
                MyAPIGateway.Utilities.RegisterMessageHandler(Channel, HandleMessage);
            }
            if (!IsReady)
                MyAPIGateway.Utilities.SendModMessage(Channel, "ApiEndpointRequest");
            return IsReady;
        }

        public bool SetPlanetInfo(MyPlanet planet, bool dayCycleEnabled, float dayLengthSeconds, bool spinCounterClockwise) => _setPlanetInfo?.Invoke(planet, dayCycleEnabled, dayLengthSeconds, spinCounterClockwise) ?? false;

        public bool SetStarInfo(MyPlanet planet, float radius, Vector3I color, float effectBrightness, float lightBrightness, float damageRadius, float gravityStrength, float gravityFalloff) => _setStarInfo?.Invoke(planet, radius, color, effectBrightness, lightBrightness, damageRadius, gravityStrength, gravityFalloff) ?? false;

        public void SpawnPlanetNoCycle(string planetName, Vector3D position, float radius, int seed) => _spawnPlanetNoCycle?.Invoke(planetName, position, radius, seed);

        public void SpawnPlanetWithCycle(string planetName, Vector3D position, float radius, int seed, float daySeconds, bool isCounterClockwise, float extraZoneRadiusKm) => _spawnPlanetWithCycle?.Invoke(planetName, position, radius, seed, daySeconds, isCounterClockwise, extraZoneRadiusKm);

        public void SpawnStar(Vector3D position, float radius, Vector3I color, float effectBrightness, float lightBrightness, float damageRadius, float gravityStrength, float gravityFalloff) => _spawnStar?.Invoke(position, radius, color, effectBrightness, lightBrightness, damageRadius, gravityStrength, gravityFalloff);

        public void Unload() {
            if (_isRegistered) {
                _isRegistered = false;
                MyAPIGateway.Utilities.UnregisterMessageHandler(Channel, HandleMessage);
            }
            IsReady = false;
        }

        private void HandleMessage(object o) {
            if (_apiInit)
                return;
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
    }
}