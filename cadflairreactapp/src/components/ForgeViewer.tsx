/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useRef } from "react";

declare const Autodesk: any

interface ForgeViewerProps {
    bucketKey?: string,
    objectKey?: string,
}

export default function ForgeViewer(props: ForgeViewerProps) {

    const viewerRef = useRef(null)

    useEffect(() => {

         if (!props.bucketKey || !props.objectKey)
                return

        //const tokenResponse = await fetch(`/api/v1/forge/viewer/token`);
        //const token = await tokenResponse.json();

        //const urnResponse = await fetch(`/api/v1/forge/buckets/${props.bucketKey}/objects/${props.objectKey}`);
        //const urn = await urnResponse.json();

        fetch(`/api/v1/forge/buckets/${props.bucketKey}/objects/${props.objectKey}/urn`)
            .then(res => res.json())
            .then(data => {
                console.log(data);

                const token = data.token;
                const urn = data.urn;

                const options = {
                    env: 'AutodeskProduction',
                    accessToken: token
                };

                let viewer:any;

                Autodesk.Viewing.Initializer(options, () => {
                    viewer = new Autodesk.Viewing.GuiViewer3D(viewerRef.current, { extensions: ['Autodesk.DocumentBrowser'] });
                    //viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('forgeViewer'), { extensions: ['Autodesk.DocumentBrowser'] });
                    viewer.addEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT, onExtensionLoaded);
                    viewer.addEventListener(Autodesk.Viewing.TOOLBAR_CREATED_EVENT, onToolbarCreated);
                    viewer.start();

                    // load from a model derivative manifest using a urn, urn must be Base64 encoded
                    Autodesk.Viewing.Document.load(`urn:${urn}`, onDocumentLoadSuccess, onDocumentLoadFailure);
                });

                const onDocumentLoadSuccess = (doc:any) => {
                    const viewables = doc.getRoot().getDefaultGeometry();
                    viewer.loadDocumentNode(doc, viewables).then(() => {
                        // documented loaded, add any additional actions here
                        console.log('Forge Viewer: onDocumentLoadSuccess()');
                    });
                }

                const onDocumentLoadFailure = (errorCode:any, errorMsg:any) =>  {
                    console.error('Forge Viewer: onDocumentLoadFailure() - errorCode:' + errorCode + '\n- errorMessage:' + errorMsg);
                }

                // event used to hide unwanted controls on the toolbar
                const onToolbarCreated = () => {
                    //console.log('on toobar created: ');
                    //for (var i = 0; i < viewer.toolbar.getNumberOfControls(); i++) {
                    //    var controlId = viewer.toolbar.getControlId(i);
                    //    var control = viewer.toolbar.getControl(controlId);
                    //    console.log(control);
                    //}

                    //const navTools = viewer.toolbar.getControl('navTools');
                    //const modelTools = viewer.toolbar.getControl('modelTools');
                    const settingsTools = viewer.toolbar.getControl('settingsTools');
                    settingsTools.removeControl('toolbar-settingsTool');
                    //settingsTools.removeControl('toolbar-fullscreenTool');

                    viewer.removeEventListener(Autodesk.Viewing.TOOLBAR_CREATED_EVENT, onToolbarCreated);
                }

                // event used to hide unwanted controls on the toolbar
                const onExtensionLoaded = (e:any) => {
                    //console.log('loaded extension: ' + e.extensionId);

                    //if (e.extensionId === 'Autodesk.DocumentBrowser') {
                    //    // this seems to be loaded before the toolbar exists so can not remove 'toolbar-documentModels' here
                    //}

                    //if (e.extensionId === 'Autodesk.MixpanelExtension') {
                    //}

                    if (e.extensionId === 'Autodesk.DefaultTools.NavTools') {
                        viewer.toolbar.getControl('navTools').removeControl('toolbar-cameraSubmenuTool');
                        viewer.toolbar.getControl('modelTools').removeControl('toolbar-documentModels'); // this is loaded after the document browser so this seems to work fine
                    }

                    if (e.extensionId === 'Autodesk.Explode') {
                        viewer.toolbar.getControl('modelTools').removeControl('toolbar-explodeTool');
                    }

                    //if (e.extensionId === 'Autodesk.Viewing.FusionOrbit') {
                    //}

                    if (e.extensionId === 'Autodesk.ModelStructure') {
                        viewer.toolbar.getControl('settingsTools').removeControl('toolbar-modelStructureTool');
                    }

                    if (e.extensionId === 'Autodesk.PropertiesManager') {
                        viewer.toolbar.getControl('settingsTools').removeControl('toolbar-propertiesTool');
                    }

                    //if (e.extensionId === 'Autodesk.ViewCubeUi') {
                    //}

                    if (e.extensionId === 'Autodesk.BimWalk') {
                        viewer.toolbar.getControl('navTools').removeControl('toolbar-bimWalkTool');
                    }

                    //if (e.extensionId === 'Autodesk.LayerManager') {
                    //}

                    //if (e.extensionId === 'Autodesk.BoxSelection') {
                    //}

                    //if (e.extensionId === 'Autodesk.CompGeom') {
                    //}

                    if (e.extensionId === 'Autodesk.Section') {
                        viewer.toolbar.getControl('modelTools').removeControl('toolbar-sectionTool');
                    }

                    //if (e.extensionId === 'Autodesk.Snapping') {
                    //}

                    //if (e.extensionId === 'Autodesk.Measure') {
                    //    viewer.toolbar.getControl('modelTools').removeControl('toolbar-measurementSubmenuTool');
                    //}

                    //viewer.removeEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT, onExtensionLoaded);
                }
            })

    }, [props.bucketKey, props.objectKey]);

    return (
        <div ref={viewerRef} style={{ position: "relative", height:"100%", width:"100%" }}></div>
    );
}
