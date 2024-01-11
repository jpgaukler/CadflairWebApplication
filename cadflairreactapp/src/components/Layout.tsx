import '@mantine/core/styles.css';
import { AppShell, Image, Button, Group, useMantineColorScheme, ActionIcon } from '@mantine/core';
import { Outlet } from 'react-router-dom';
import logo from '../assets/cadflair_logo.svg';
import logoDark from '../assets/cadflair_logo_dark.svg';


export default function Layout() {
    const { setColorScheme, clearColorScheme } = useMantineColorScheme()

    return (
        <AppShell
            header={{ height: 60 }}
            padding="md"
        >
            <AppShell.Header>
                <Group h="100%" px="md" justify="space-between">
                    <a href="/" style={{ height: "100%" }}>
                        <Image src={logoDark} alt="Cadflair Logo" h="100%" p="xs" />
                    </a>
                    <Group>
                        <Button>Link 1</Button>
                        <Button>Link 2</Button>
                        <Button>Link 3</Button>
                    {/*    <ActionIcon*/}
                    {/*        onClick={() => setColorScheme(computedColorScheme === 'light' ? 'dark' : 'light')}*/}
                    {/*        variant="default"*/}
                    {/*        size="xl"*/}
                    {/*        aria-label="Toggle color scheme"*/}
                    {/*    >*/}
                    {/*        <IconSun className={cx(classes.icon, classes.light)} stroke={1.5} />*/}
                    {/*        <IconMoon className={cx(classes.icon, classes.dark)} stroke={1.5} />*/}
                    {/*    </ActionIcon>*/}
                    </Group>
                    <Group>
                        <Button onClick={() => setColorScheme('light')}>Light</Button>
                        <Button onClick={() => setColorScheme('dark')}>Dark</Button>
                        {/*<Button onClick={() => setColorScheme('auto')}>Auto</Button>*/}
                        {/*<Button onClick={clearColorScheme}>Clear</Button>*/}
                    </Group>
                </Group>
            </AppShell.Header>
            <AppShell.Main>
                <Outlet />
            </AppShell.Main>
            <AppShell.Footer>
                Footer
            </AppShell.Footer>
        </AppShell>
    );
}
