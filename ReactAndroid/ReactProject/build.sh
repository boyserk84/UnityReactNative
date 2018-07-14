ASSETS_DIR="android/app/src/main/assets/"

if [[ ! -d "android/app/src/main/assets/" ]]; then
	echo "Create assets directory: '${ASSETS_DIR}'."
	mkdir "$ASSETS_DIR"
fi

echo "Building Android bundle."
npm run bundle-android


# copy index.android.bundle into native folder so it knows where to load
ANDROID_PROJ_ASSET="native_android/app/src/main/assets/"
if [[ ! -d "native_android/app/src/main/assets/" ]]; then
	echo "Create assets directory: '${ANDROID_PROJ_ASSET}'."
	mkdir "$ANDROID_PROJ_ASSET"
fi


cp "${ASSETS_DIR}/index.android.bundle" "${ANDROID_PROJ_ASSET}/index.android.bundle"
