import '@mantine/core/styles.css';
import { Text, AppShell, Image, Button, Group, Burger, Container, Grid, Stack, Anchor, Box, NavLink } from '@mantine/core';
import { Outlet } from 'react-router-dom';
import logoDark from '../assets/cadflair_logo_dark.svg';
import { useDisclosure } from '@mantine/hooks';


export default function Layout() {
    const [opened, { toggle }] = useDisclosure();

    return (
        <AppShell
            header={{ height: 60 }}
            aside={{ width: 300, breakpoint: 'sm', collapsed: { mobile: !opened, desktop:true } }}
            footer={{ height: { base: 770, xs:600, sm: 500, md: 400, lg: 300 }, offset: true }}
        >
            <AppShell.Header bg="dark.8">
                <Group h="100%" px="md" justify="space-between">
                    <a href="/" style={{ height: "100%" }}>
                        <Image src={logoDark} alt="Cadflair Logo" h="100%" p="xs" />
                    </a>
                    <Group gap="3px" visibleFrom="sm">
                        <a href="/">
                            <Button variant="subtle" c="white">Home</Button>
                        </a>
                        <a href="demo/categories">
                            <Button variant="subtle" c="white">Demo</Button>
                        </a>
                        <a href="/#contact-us">
                            <Button variant="subtle">Contact Us</Button>
                        </a>
                    </Group>
                    <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" color="white" />
                </Group>
            </AppShell.Header>
            <AppShell.Aside h="100%">
                <NavLink href="/" label="Home" />
                <NavLink href="/demo/categories" label="Demo" />
                <NavLink href="/#contact-us" label="Contact Us" />
            </AppShell.Aside>
            <AppShell.Main >
                <Box style={{ zIndex: 10, position: "relative" }} bg="#ffffff" mih="100vh">
                    <Outlet />
                </Box>
            </AppShell.Main>
            <AppShell.Footer zIndex={1} bg="dark.8">
                <Container size="lg" h="100%">
                    <Stack h="100%" justify="center">
                        <Grid>
                            <Grid.Col span={{ sm: 12, md: 4 }}>
                                <Image src={logoDark} alt="Cadflair Logo" maw={275} mb="lg" />
                            </Grid.Col>

                            <Grid.Col span={{ xs: 6, md: 2 }}>
                                <Stack>
                                    <Text tt="uppercase" fw={600} c="white" lts={1.35}>Why Cadflair?</Text>
                                    <Anchor href="/demo/categories">Product Demo</Anchor>
                                    <Anchor href="/comingsoon">Customer Testimonies</Anchor>
                                    <Anchor href="/comingsoon">FAQ</Anchor>
                                </Stack>
                            </Grid.Col>

                            <Grid.Col span={{ xs: 6, md: 2 }}>
                                <Stack>
                                    <Text tt="uppercase" fw={600} c="white" lts={1.35}>Product</Text>
                                    <Anchor href="/comingsoon">Pricing</Anchor>
                                    <Anchor href="/comingsoon">Features</Anchor>
                                    <Anchor href="/comingsoon">Technology</Anchor>
                                    <Anchor href="/comingsoon">Releases</Anchor>
                                </Stack>
                            </Grid.Col>

                            <Grid.Col span={{ xs: 6, md: 2 }}>
                                <Stack>
                                    <Text tt="uppercase" fw={600} c="white" lts={1.35}>Resources</Text>
                                    <Anchor href="/comingsoon">Documentation</Anchor>
                                    <Anchor href="/comingsoon">Tutorials</Anchor>
                                </Stack>
                            </Grid.Col>

                            <Grid.Col span={{ xs: 6, md: 2 }}>
                                <Stack>
                                    <Text tt="uppercase" fw={600} c="white" lts={1.35}>Company</Text>
                                    <Anchor href="/comingsoon">About Us</Anchor>
                                    <Anchor href="/comingsoon">News</Anchor>
                                </Stack>
                            </Grid.Col>
                        </Grid>
                    </Stack>
                </Container>
            </AppShell.Footer>
        </AppShell>
    );
}
