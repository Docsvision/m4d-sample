import IMask from 'imask';

export const customizeInputFields = () => {
    const codeTaxAuthSubmit = document.querySelector('[data-control-name="codeTaxAuthSubmit"]');
    codeTaxAuthSubmit?.getElementsByTagName('input')[0].setAttribute("maxLength", "4");
    const codeTaxAuthValid = document.querySelector('[data-control-name="codeTaxAuthValid"]');
    codeTaxAuthValid?.getElementsByTagName('input')[0].setAttribute("maxLength", "4");
    const ceoINN = document.querySelector('[data-control-name="ceoINN"]');
    ceoINN?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const ceoCitizenship = document.querySelector('[data-control-name="ceoCitizenship"]');
    ceoCitizenship?.getElementsByTagName('input')[0].setAttribute("maxLength", "3");
    const ceoAddrSubRus = document.querySelector('[data-control-name="ceoAddrSubRus"]');
    ceoAddrSubRus?.getElementsByTagName('input')[0].setAttribute("maxLength", "3");
    const reprINN = document.querySelector('[data-control-name="reprINN"]');
    reprINN?.getElementsByTagName('input')[0].setAttribute("maxLength", "12");
    const reprCitizenship = document.querySelector('[data-control-name="reprCitizenship"]');
    reprCitizenship?.getElementsByTagName('input')[0].setAttribute("maxLength", "3");
    const reprAddrSubRus = document.querySelector('[data-control-name="reprAddrSubRus"]');
    reprAddrSubRus?.getElementsByTagName('input')[0].setAttribute("maxLength", "2");
    const maskOptions = {
        mask: '000-000-000 00'
    }
    const ceoSNILSInputElement = document.querySelector('[data-control-name="ceoSNILS"]')?.getElementsByTagName('input')[0];
    IMask(ceoSNILSInputElement, maskOptions)
    const reprSNILSInputElement = document.querySelector('[data-control-name="reprSNILS"]')?.getElementsByTagName('input')[0];
    IMask(reprSNILSInputElement, maskOptions)
}