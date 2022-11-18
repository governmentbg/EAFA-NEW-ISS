import {
    faFilter,
    faRedo,
    faQuestion,
    faPlusCircle,
    faPlus,
    faShip,
    faGavel,
    faHandshake,
    faCogs,
    faBook,
    faArchive,
    faFileAlt,
    faThList,
    faClipboardCheck,
    faFish,
    faTractor,
    faUser,
    faUserTie,
    faNewspaper,
    faUsersCog,
    faCertificate,
    faChartLine,
    faBalanceScale,
    faMoneyBillAlt,
    faUserShield,
    faHashtag,
    faAward,
    faFileSignature,
    faBug,
    faStamp,
    faCalendar,
    faCalendarAlt,
    faHourglassHalf,
    faPlusSquare,
    faBookOpen,
    faAddressBook,
    faSdCard,
    faGlobe,
    faDatabase,
    faPeopleArrows,
    faCheckDouble,
    faUserPlus,
    faTicketAlt,
    faUsers,
    faHandPointUp,
    faFlask,
    faSync,
    faVest,
    faWeight,
    faStickyNote,
    faUserGraduate,
    faListAlt,
    faConciergeBell,
    faTrash,
    faSave,
    faUndo,
    faEdit,
    faServer,
    faIdBadge,
    faHourglassEnd,
    faSearch,
    faExternalLinkAlt,
    faCheck,
    faTimes,
    faKey,
    faEye,
    faEyeSlash,
    faSignInAlt,
    faBan,
    faIdCard,
    faWheelchair,
    faBookReader,
    faChild,
    faBaby,
    faUserAlt,
    faBinoculars,
    faAnchor,
    faExchangeAlt,
    faDolly,
    faWarehouse,
    faWater,
    faTag,
    faShippingFast,
    faStore,
    faTachometerAlt,
    faChartBar,
    faMapMarkerAlt,
    faDownload,
    faReply,
    faEnvelope,
    faComment,
    faEnvelopeOpen,
    faClipboardList,
    faFileExport,
    faSignature
} from '@fortawesome/free-solid-svg-icons';

import { faAddressBook as faAddressBookRegular } from '@fortawesome/free-regular-svg-icons';

import deleteIcon from '@iconify/icons-mdi/delete-forever';
import restoreIcon from '@iconify/icons-mdi/delete-restore';
import editIcon from '@iconify/icons-mdi/edit';
import filterRemove from '@iconify/icons-mdi/filter-remove';
import groupAdd from '@iconify/icons-mdi/group-add';
import home from '@iconify/icons-mdi/home';
import fileAdd from '@iconify/icons-mdi/file-plus-outline';
import plusCircle from '@iconify/icons-mdi/plus-circle';
import hook from '@iconify/icons-mdi/hook';
import download from '@iconify/icons-mdi/download';
import downloadOff from '@iconify/icons-mdi/download-off';
import newspaperMultipleOutline from '@iconify/icons-mdi/newspaper-variant-multiple-outline';
import emailCheckOutline from '@iconify/icons-mdi/email-check-outline';
import viewDashboard from '@iconify/icons-mdi/view-dashboard';
import contentCopy from '@iconify/icons-mdi/content-copy';
import mdiExclamationMark from '@iconify/icons-mdi/exclamation';
import fishbowl from '@iconify/icons-mdi/fishbowl';
import documentArrowRight24Regular from '@iconify-icons/fluent/document-arrow-right-24-regular';
import documentSync24Regular from '@iconify-icons/fluent/document-sync-24-regular';
import documentProhibited24Regular from '@iconify-icons/fluent/document-prohibited-24-regular';
import documentQuestionMark24Regular from '@iconify-icons/fluent/document-question-mark-24-regular';
import documentPerson20Regular from '@iconify-icons/fluent/document-person-20-regular';
import documentCheckmark24Regular from '@iconify-icons/fluent/document-checkmark-24-regular';
import personMoney24Filled from '@iconify-icons/fluent/person-money-24-filled';
import personProhibited28Filled from '@iconify-icons/fluent/person-prohibited-28-filled';
import documentEdit16Regular from '@iconify-icons/fluent/document-edit-16-regular';
import documentBriefcase24Regular from '@iconify-icons/fluent/document-briefcase-24-regular';
import document28Regular from '@iconify-icons/fluent/document-28-regular';
import documentError16Regular from '@iconify-icons/fluent/document-error-16-regular';
import arrowDownload16Filled from '@iconify-icons/fluent/arrow-download-16-filled';
import arrowUpload16Filled from '@iconify-icons/fluent/arrow-upload-16-filled';
import signed16Regular from '@iconify-icons/fluent/signed-16-regular';
import editArrowBack16Regular from '@iconify-icons/fluent/edit-arrow-back-16-regular';
import warning24Filled from '@iconify-icons/fluent/warning-24-filled';
import cancel from '@iconify/icons-mdi/cancel';
import buildingRetailMoney20Regular from '@iconify-icons/fluent/building-retail-20-regular';

export class AppIcons {
    /*
     * Всички икони от тази колекция трябва да имат префикс 'ic-'
     */
    public static readonly IC_ICONS = {
        'ic-edit': editIcon,
        'ic-delete': deleteIcon,
        'ic-restore': restoreIcon,
        'ic-home': home,
        'ic-group-add': groupAdd,
        'ic-filter-remove': filterRemove,
        'ic-file-add': fileAdd,
        'ic-plus-circle': plusCircle,
        'ic-hook': hook,
        'ic-download': download,
        'ic-download-off': downloadOff,
        'ic-newspaper-variant-multiple-outline': newspaperMultipleOutline,
        'ic-email-check': emailCheckOutline,
        'ic-dashboard': viewDashboard,
        'ic-copy': contentCopy,
        'ic-exclamation': mdiExclamationMark,
        'ic-fishbowl': fishbowl,
        'ic-fluent-doc-arrow-right-24-regular': documentArrowRight24Regular,
        'ic-fluent-doc-sync-24-regular': documentSync24Regular,
        'ic-fluent-doc-prohibited-24-regular': documentProhibited24Regular,
        'ic-fluent-doc-question-mark-24-regular': documentQuestionMark24Regular,
        'ic-fluent-doc-person-20-regular': documentPerson20Regular,
        'ic-fluent-doc-checkmark-24-regular': documentCheckmark24Regular,
        'ic-fluent-person-money-24-filled': personMoney24Filled,
        'ic-fluent-person-prohibited-28-filled': personProhibited28Filled,
        'ic-fluent-document-edit-16-regular': documentEdit16Regular,
        'ic-fluent-document-briefcase-24-regular': documentBriefcase24Regular,
        'ic-fluent-document-28-regular': document28Regular,
        'ic-fluent-document-error-16-regular': documentError16Regular,
        'ic-fluent-arrow-download-16-filled': arrowDownload16Filled,
        'ic-fluent-arrow-upload-16-filled': arrowUpload16Filled,
        'ic-fluent-signed-16-regular': signed16Regular,
        'ic-fluent-edit-arrow-back-16-regular': editArrowBack16Regular,
        'ic-fluent-warning-24-filled': warning24Filled,
        'ic-cancel': cancel,
        'ic-building-retail-money-20-regular': buildingRetailMoney20Regular
    };

    /*
     * Всички икони от тази колекция трябва да имат префикс 'fa-'
     */
    public static readonly FA_ICONS = {
        'fa-sign-in': faSignInAlt,
        'fa-filter': faFilter,
        'fa-redo': faRedo,
        'fa-question': faQuestion,
        'fa-plus-circle': faPlusCircle,
        'fa-plus': faPlus,
        'fa-ship': faShip,
        'fa-gavel': faGavel,
        'fa-handshake': faHandshake,
        'fa-cogs': faCogs,
        'fa-book': faBook,
        'fa-archive': faArchive,
        'fa-file-alt': faFileAlt,
        'fa-th-list': faThList,
        'fa-clipboard-check': faClipboardCheck,
        'fa-fish': faFish,
        'fa-tractor': faTractor,
        'fa-user': faUser,
        'fa-user-tie': faUserTie,
        'fa-newspaper': faNewspaper,
        'fa-users-cog': faUsersCog,
        'fa-certificate': faCertificate,
        'fa-chart-line': faChartLine,
        'fa-balance-scale': faBalanceScale,
        'fa-money-bill-alt': faMoneyBillAlt,
        'fa-user-shield': faUserShield,
        'fa-hashtag': faHashtag,
        'fa-award': faAward,
        'fa-file-signature': faFileSignature,
        'fa-bug': faBug,
        'fa-stamp': faStamp,
        'fa-calendar-alt': faCalendarAlt,
        'fa-hourglass-half': faHourglassHalf,
        'fa-plus-square': faPlusSquare,
        'fa-book-open': faBookOpen,
        'fa-id-badge': faIdBadge,
        'fa-address-book': faAddressBook,
        'fa-address-book-regular': faAddressBookRegular,
        'fa-sd-card': faSdCard,
        'fa-globe': faGlobe,
        'fa-database': faDatabase,
        'fa-people-arrows': faPeopleArrows,
        'fa-check-double': faCheckDouble,
        'fa-user-plus': faUserPlus,
        'fa-ticket-alt': faTicketAlt,
        'fa-users': faUsers,
        'fa-handpoint-up': faHandPointUp,
        'fa-flask': faFlask,
        'fa-sync': faSync,
        'fa-vest': faVest,
        'fa-weight': faWeight,
        'fa-sticky-note': faStickyNote,
        'fa-user-graduate': faUserGraduate,
        'fa-list-alt': faListAlt,
        'fa-concierge-bell': faConciergeBell,
        'fa-trash': faTrash,
        'fa-save': faSave,
        'fa-undo': faUndo,
        'fa-edit': faEdit,
        'fa-server': faServer,
        'fa-hourglass-end': faHourglassEnd,
        'fa-search-icon': faSearch,
        'fa-external-link-alt': faExternalLinkAlt,
        'fa-check': faCheck,
        'fa-key': faKey,
        'fa-times': faTimes,
        'fa-eye': faEye,
        'fa-eye-slash': faEyeSlash,
        'fa-ban': faBan,
        'fa-id-card': faIdCard,
        'fa-wheelchair': faWheelchair,
        'fa-book-reader': faBookReader,
        'fa-child': faChild,
        'fa-baby': faBaby,
        'fa-user-alt': faUserAlt,
        'fa-binoculars': faBinoculars,
        'fa-anchor': faAnchor,
        'fa-exchange-alt': faExchangeAlt,
        'fa-dolly': faDolly,
        'fa-warehouse': faWarehouse,
        'fa-water': faWater,
        'fa-tag': faTag,
        'fa-shipping-fast': faShippingFast,
        'fa-store': faStore,
        'fa-tachometer-alt': faTachometerAlt,
        'fa-chart-bar': faChartBar,
        'fa-map-marker-alt': faMapMarkerAlt,
        'fa-download': faDownload,
        'fa-reply': faReply,
        'fa-envelope': faEnvelope,
        'fa-comment': faComment,
        'fa-envelope-open': faEnvelopeOpen,
        'fa-clipboard-list': faClipboardList,
        'fa-file-export': faFileExport,
        'fa-signature': faSignature,
    };

    public static get FaIconsDictionary(): Map<string, any> {
        if (AppIcons.FA_ICONS_ARRAY == undefined) {
            const dictionary: Map<string, any> = new Map<string, any>();
            const keys = Object.keys(this.FA_ICONS);
            const values = Object.values(this.FA_ICONS);

            for (let i = 0; i < keys.length; i++) {
                dictionary.set(keys[i], values[i]);
            }

            AppIcons.FA_ICONS_ARRAY = dictionary;
        }

        return AppIcons.FA_ICONS_ARRAY;
    }

    private static FA_ICONS_ARRAY: Map<string, any>;

    public static get IcIconsDictionary(): Map<string, any> {
        if (AppIcons.IC_ICONS_ARRAY == undefined) {
            const dictionary: Map<string, any> = new Map<string, any>();
            const keys = Object.keys(this.IC_ICONS);
            const values = Object.values(this.IC_ICONS);

            for (let i = 0; i < keys.length; i++) {
                dictionary.set(keys[i], values[i]);
            }

            AppIcons.IC_ICONS_ARRAY = dictionary;
        }

        return AppIcons.IC_ICONS_ARRAY;
    }

    private static IC_ICONS_ARRAY: Map<string, any>;
}