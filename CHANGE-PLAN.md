# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.addressables-toolbox` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- Replaced `ComponentReference` with a new implementation written from Unity's public Addressables API. The earlier version was based on a Unity sample without a stated license.
- Added PlayMode tests (`Tests/PlayMode`) that run `ComponentReference` against real Addressables: loading, the missing-component failure, release and reload, and editor validation.
- Made standalone: no longer declares Addressables or UniTask as dependencies. Without both installed the package compiles to nothing; `ImageExtensions` also needs uGUI.

## Planned changes

- [x] Tag and publish `v0.6.0` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
- [ ] Consider `InstantiateAsync` helpers on `ComponentReference` (no package needs them today).
<!-- review-items:start -->
- [ ] **P1** Make package-core optional: find what uses it, add small stand-ins behind `TEASPOONS_PACKAGE_CORE` (the pattern from `logging` 1.4.0 and `static-data` 0.23.0) and drop it from `package.json`.
- [ ] **P1** Tie `LoadImage` to the image's lifetime (`GetCancellationTokenOnDestroy`) and release the handle when the load is cancelled or the image is gone. Add a PlayMode test that destroys the `Image` mid-load.
- [ ] **P1** Declares `unity: 6000.0`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P2** Document the reference-counting contract in the README: who calls `ReleaseAsset`, and what happens on scene unload.
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
- [ ] **P2** The README is only 48 lines. Add a short example for each public type.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

No close open-source comparable turned up in this pass, so the findings below come from reading the code.

### Findings from reading the code

- **[Lifecycle]** `ImageExtensions.LoadImage` starts `LoadImageAsync(...).Forget()` with no cancellation tied to the `Image`. If the image is destroyed while the sprite loads, only the `if (image)` check protects the assignment, and nothing releases the loaded asset. The ownership of the release is unclear.
- **[Coupling]** It still declares `com.tea-spoons.package-core`. `stacking-dialogs` and `ugui-design-system` reach package-core through this package, so making it optional (as done for logging, static-data and stacking-dialogs) is the change with the most leverage.
- **[Good]** `ComponentReference<T>.ReleaseAsset` releases both the component handle and the prefab handle, and there are PlayMode tests (3 files, 220 lines). They do not run in CI yet.
<!-- review:end -->

## Notes and ideas

_Add your own here._
