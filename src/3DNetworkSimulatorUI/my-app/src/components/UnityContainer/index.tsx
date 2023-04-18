import React from "react";
import { Unity, useUnityContext } from "react-unity-webgl";

const UnityContainer: React.FC = () => {
    const { unityProvider } = useUnityContext({
        loaderUrl: "/build/Comp.loader.js",
        dataUrl: "/build/webgl.data",
        frameworkUrl: "/build/build.framework.js",
        codeUrl: "/build/build.wasm",
    });
    return < Unity unityProvider={unityProvider} />;
}

export default UnityContainer