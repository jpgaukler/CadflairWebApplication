import '@mantine/core/styles.css';
import { AppShell, Burger, Button, Group, MantineProvider, Text } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Outlet } from 'react-router-dom';

export default function Layout() {
    const [opened, { toggle }] = useDisclosure();

    return (
        <MantineProvider>
            <AppShell
                header={{ height: 60 }}
                padding="md"
            >
                <AppShell.Header>
                    <Group h="100%" px="md" justify="space-between">
                        <Text size="xl" fw="800">Cadflair Logo</Text>
                        <Group>
                            <Button>Link 1</Button>
                            <Button>Link 2</Button>
                            <Button>Link 3</Button>
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
        </MantineProvider>
    );
}
