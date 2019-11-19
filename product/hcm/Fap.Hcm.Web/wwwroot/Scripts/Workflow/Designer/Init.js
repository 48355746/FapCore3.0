// urlParams is null when used for embedding
window.urlParams = window.urlParams || {};

// Public global variables
window.MAX_REQUEST_SIZE = window.MAX_REQUEST_SIZE  || 10485760;
window.MAX_AREA = window.MAX_AREA || 15000 * 15000;

// URLs for save and export
window.EXPORT_URL = window.EXPORT_URL || '/export';
window.SAVE_URL = window.SAVE_URL || '/api/Workflow/ProcessApi/SaveWorkflowDiagraph';
window.SAVEAS_URL = window.SAVEAS_URL || '/Workflow/Designer/SaveFile';
window.OPEN_URL = window.OPEN_URL || '/api/Workflow/ProcessApi/SaveWorkflowDiagraph';
//流转设置
window.TRANSITION_URL = window.TRANSITION_URL || '/Workflow/Designer/Transition';
//单据模板
window.BILLTEMPLATE_URL = window.BILLTEMPLATE_URL || '/api/Workflow/ProcessApi/GetBillTemplate';
//单据字段
window.FIELDLIST_URL = window.FIELDLIST_URL || '/api/coreapi/fieldlist';
//子流程模板
window.SUBPROCESS_URL = window.SUBPROCESS_URL || '/api/Workflow/ProcessApi/GetWfTemplate';
//申请人选择
window.APPROVER_URL = window.APPROVER_URL || '/Workflow/Designer/Approver';
window.RESOURCES_PATH = window.RESOURCES_PATH || '../../../Content/mxGraph/resources';
window.RESOURCE_BASE = window.RESOURCE_BASE || window.RESOURCES_PATH + '/grapheditor';
window.STENCIL_PATH = window.STENCIL_PATH || '../../../Content/mxGraph/stencils';
window.IMAGE_PATH = window.IMAGE_PATH || '../../../Content/mxGraph/images';
window.STYLE_PATH = window.STYLE_PATH || '../../../Content/mxGraph/styles';
window.CSS_PATH = window.CSS_PATH || '../../../Content/mxGraph/styles';
window.OPEN_FORM = window.OPEN_FORM || 'OpenFile';

// Sets the base path, the UI language via URL param and configures the
// supported languages to avoid 404s. The loading of all core language
// resources is disabled as all required resources are in grapheditor.
// properties. Note that in this example the loading of two resource
// files (the special bundle and the default bundle) is disabled to
// save a GET request. This requires that all resources be present in
// each properties file since only one file is loaded.
window.mxBasePath = window.mxBasePath || '../../../Content/mxGraph/src';
//window.mxLanguage = window.mxLanguage || urlParams['lang'];
window.mxLanguage = window.mxLanguage || 'zh';
window.mxLanguages = window.mxLanguages || ['zh'];
