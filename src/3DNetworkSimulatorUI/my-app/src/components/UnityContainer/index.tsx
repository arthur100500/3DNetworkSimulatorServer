import React from "react";
import { Unity, useUnityContext } from "react-unity-webgl";

const UnityContainer: React.FC = () => {
    const { unityProvider } = useUnityContext({
        loaderUrl: "/build/Build/build.loader.js",
        dataUrl: "/build/Build/webgl.data",
        frameworkUrl: "/build/Build/build.framework.js",
        codeUrl: "/build/Build/build.wasm",
    });
    return < Unity unityProvider={unityProvider} />;
}

export default UnityContainer