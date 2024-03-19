# [1.0.11](https://github.com/SiskSjet/SmartRotors/compare/v1.0.10...v1.0.11) (2024-03-19)


### Bug Fixes

* limits were accidentally set on the wrong rotor ([c803efd](https://github.com/SiskSjet/SmartRotors/commit/c803efde03755b27f316c1e199bc6e67a88a2554))


### Features

* add support for `Real Stars` ([333723d](https://github.com/SiskSjet/SmartRotors/commit/333723d10b793a69436bcc37489123b3d7203852))



# [1.0.10](https://github.com/SiskSjet/SmartRotors/compare/v1.0.9...v1.0.10) (2023-04-13)


### Bug Fixes

* fix ctd after se update ([e8818c5f259ce881c1aa82108319fac2ead9bda9](https://github.com/SiskSjet/SmartRotors/commit/e8818c5f259ce881c1aa82108319fac2ead9bda9))



# [1.0.9](https://github.com/SiskSjet/SmartRotors/compare/v1.0.8...v1.0.9) (2020-11-30)


### Bug Fixes

* fix tracking issues ([c55d85c](https://github.com/SiskSjet/SmartRotors/commit/c55d85c50e7a4e1b138f4b56c1e2b35633db6c32))



# [1.0.7](https://github.com/SiskSjet/SmartRotors/compare/v1.0.6...v1.0.7) (2020-06-25)


### Bug Fixes

* fix an issue which prevent the rotor head of the hinge to spawn ([35d2091](https://github.com/SiskSjet/SmartRotors/commit/35d20911cb1393845db1f76588ef735b0194a005))



# [1.0.6](https://github.com/SiskSjet/SmartRotors/compare/v1.0.5...v1.0.6) (2020-04-08)


### Bug Fixes

* fix NullReferenceException introduced with latest hotfix ([cc0bb41](https://github.com/SiskSjet/SmartRotors/commit/cc0bb419c2ca9f43b0a277cd7ac2e35cedbd7508))



# [1.0.5](https://github.com/SiskSjet/SmartRotors/compare/v1.0.4...v1.0.5) (2020-04-06)


### Bug Fixes

* fix * is not in the same block-variant group error ([eda9fed](https://github.com/SiskSjet/SmartRotors/commit/eda9fed896290040f9764dfbffef63c933d3b177))
* fix an issue where scenario task would not work after a reload ([3ff7fce](https://github.com/SiskSjet/SmartRotors/commit/3ff7fce8932326ba746a74e24c54dbc7770bd226))
* fix BlockVariants are obsolete warning ([7aae20b](https://github.com/SiskSjet/SmartRotors/commit/7aae20bed01075ea4f854562d1758b1e1c903eb5))



# [1.0.4](https://github.com/SiskSjet/SmartRotors/compare/v1.0.3...v1.0.4) (2019-06-06)


* fix: fix exception with last update of SE ([0102eb7](https://github.com/SiskSjet/SmartRotors/commit/0102eb7))


### Features

* feat: add support for the new skin feature ([898ddab](https://github.com/SiskSjet/SmartRotors/commit/898ddab))



# [1.0.3](https://github.com/SiskSjet/SmartRotors/compare/v1.0.2...v1.0.3) (2019-05-09)


### Bug Fixes

* fix a speculative issue which may causes the rare crashes ([c42fa54](https://github.com/SiskSjet/SmartRotors/commit/c42fa54)), closes [#7](https://github.com/SiskSjet/SmartRotors/issues/7)
* fix hard to weld or grind bases and parts ([27178df](https://github.com/SiskSjet/SmartRotors/commit/27178df))
* fix the support blockpairname conflict with the sloped window. ([6380b83](https://github.com/SiskSjet/SmartRotors/commit/6380b83))



# [1.0.2](https://github.com/SiskSjet/SmartRotors/compare/v1.0.1...v1.0.2) (2019-05-06)


### Bug Fixes

* predict sun position ~1.6 sec ahead ([bc3613a](https://github.com/SiskSjet/SmartRotors/commit/bc3613a)), closes [#5](https://github.com/SiskSjet/SmartRotors/issues/5)



# [1.0.1](https://github.com/SiskSjet/SmartRotors/compare/v1.0.0...v1.0.1) (2019-05-04)


### Bug Fixes

* color hinge and hinge part like the base rotor when build ([51a2108](https://github.com/SiskSjet/SmartRotors/commit/51a2108)), closes [#4](https://github.com/SiskSjet/SmartRotors/issues/4)



# 1.0.0 (2019-05-03)


### Bug Fixes

* fix an NRE on clients ([2c1d58a](https://github.com/SiskSjet/SmartRotors/commit/2c1d58a))
* fix an NRE when copy paste large amounts of grids ([dea851b](https://github.com/SiskSjet/SmartRotors/commit/dea851b))
* fix auto place issues in multiplayer ([c774874](https://github.com/SiskSjet/SmartRotors/commit/c774874))
* fix hinge placement construction with creative tools ([357b02e](https://github.com/SiskSjet/SmartRotors/commit/357b02e))
* fix load issues with solar support models ([1d91839](https://github.com/SiskSjet/SmartRotors/commit/1d91839))
* fix log location for clients ([2defbc4](https://github.com/SiskSjet/SmartRotors/commit/2defbc4))
* fix some NRE's related to multiplayer ([44cf184](https://github.com/SiskSjet/SmartRotors/commit/44cf184))
* fix the issue where the game tries to place hinges when loaded ([ec7fa58](https://github.com/SiskSjet/SmartRotors/commit/ec7fa58))
* hide and disable reverse control from solar rotors ([cda49fd](https://github.com/SiskSjet/SmartRotors/commit/cda49fd))
* remove hack to detect top part changes because keen fixed the issue ([b3631f7](https://github.com/SiskSjet/SmartRotors/commit/b3631f7))


### Features

* add a variant to solar rotor and update all models ([8d487e5](https://github.com/SiskSjet/SmartRotors/commit/8d487e5))
* add localization support and german translation ([55f2646](https://github.com/SiskSjet/SmartRotors/commit/55f2646))
* automatic placement of hinge block when the base is being built ([afaee3d](https://github.com/SiskSjet/SmartRotors/commit/afaee3d))
* create blueprint classes ([ae27075](https://github.com/SiskSjet/SmartRotors/commit/ae27075))
* hide velocity related controls and actions from solar rotors ([3dd92ca](https://github.com/SiskSjet/SmartRotors/commit/3dd92ca))
* implement sun tracking for Smart Solar Rotors ([7048a72](https://github.com/SiskSjet/SmartRotors/commit/7048a72))
* make it compatible with the research system ([86d8ce9](https://github.com/SiskSjet/SmartRotors/commit/86d8ce9))









