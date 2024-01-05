import '@mantine/core/styles.css';
import { AppShell, Burger, Group, MantineProvider, Skeleton } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Outlet } from 'react-router-dom';

export default function Layout() {
    const [opened, { toggle }] = useDisclosure();

    return (
        <MantineProvider>
            <AppShell
                header={{ height: 60 }}
                navbar={{ width: 300, breakpoint: 'sm', collapsed: { mobile: !opened } }}
                padding="md"
            >
                <AppShell.Header>
                    <Group h="100%" px="md">
                        <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
                    </Group>
                </AppShell.Header>
                <AppShell.Navbar p="md">
                    Navbar
                    {Array(15)
                        .fill(0)
                        .map((_, index) => (
                            <Skeleton key={index} h={28} mt="sm" animate={true} />
                        ))}
                </AppShell.Navbar>
                <AppShell.Main>
                    <Outlet />
                </AppShell.Main>
                <AppShell.Footer>
                    Footer
                </AppShell.Footer>
            </AppShell>
        </MantineProvider>
    );
}
