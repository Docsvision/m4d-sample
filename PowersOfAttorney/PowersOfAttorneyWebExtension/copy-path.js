const ROOT = process.env.SamplesOutput || "./../SamplesOutput"
const SITE_ROOT = `${ROOT}/Site`;
const EXTENSION_NAME = "PowersOfAttorneyWebExtension";
const MODULES_DIR = SITE_ROOT + "/Content/Modules";
const PLUGINS = [`src/DocumentSignBatchOperation/DocumentSignBatchOperationDesignerExtension/DocumentSignBatchOperationControl.xml`];
const PLUGINS_DIR = `${ROOT}/Plugins`;


module.exports.STYLES_DIR =  `${MODULES_DIR}/${EXTENSION_NAME}`;
module.exports.BUNDLE_DIR =  `${MODULES_DIR}/${EXTENSION_NAME}`;
module.exports.PLUGINS = PLUGINS;
module.exports.PLUGINS_DIR = PLUGINS_DIR;