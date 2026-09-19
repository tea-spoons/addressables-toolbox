# Addressables Toolbox
A collection of helpers to better handle the addressables system.

## Smart References
Smart References are wrappers for `AssetReference`s.
They can be used to repeatedly access a referenced object via `GetAssetAsync`.
This method will load the referenced object if needed, but won't load twice, which would cause issues in the Addressables system.

Use the subtypes `SmartAssetReference` for assets and `SmartComponentReference` for referencing prefabs via component.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/addressables-toolbox.git
```

Pin a release by appending a tag, for example `#v0.6.0`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.unity.addressables` 2.2.2
- `com.cysharp.unitask` 2.5.0
- `com.tea-spoons.package-core` 1.2.0

## Optional packages

This package works on its own. It uses the packages below when your project has them (Unity detects them automatically) and simply leaves the related code out when it does not.

| Package | Used for |
|---|---|
| Addressables (`com.unity.addressables` 2.2.2+) | The whole package. Without it nothing is compiled. |
| UniTask (`com.cysharp.unitask` 2.5.0+) | The whole package (loading is asynchronous through UniTask). Without it nothing is compiled. |
| uGUI (`com.unity.ugui`) | `ImageExtensions`. |

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
