import { FuseConfig } from '@fuse/types';

/**
 * Default Fuse Configuration
 *
 * You can edit these options to change the default options. All these options also can be
 * changed per component basis. See `app/main/pages/authentication/login/login.component.ts`
 * constructor method to learn more about changing these options per component basis.
 */

export const fuseConfig: FuseConfig = {
    // Color themes can be defined in src/app/app.theme.scss
    colorTheme: 'theme-iara', // 'theme-default',
    customScrollbars: true,
    layout: {
        style: 'vertical-layout-1',
        width: 'fullwidth',
        navbar: {
            primaryBackground: 'fuse-white-500',
            secondaryBackground: 'fuse-white-500',
            folded: false,
            hidden: false,
            position: 'left',
            variant: 'vertical-style-2'
        },
        toolbar: {
            customBackgroundColor: true,
            background: 'iara-blue-700',
            hidden: false,
            position: 'below-static'
        },
        footer: {
            customBackgroundColor: true,
            background: 'iara-blue-700',
            hidden: true,
            position: 'below-fixed'
        },
        sidepanel: {
            hidden: true,
            position: 'right'
        }
    }
};