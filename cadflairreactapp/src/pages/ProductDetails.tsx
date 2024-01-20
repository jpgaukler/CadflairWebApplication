import { Text, Group, Stack, Title, Table, Loader, Select, Button, Flex, Modal, CopyButton, TextInput, Anchor, Grid, ActionIcon, Box, em } from '@mantine/core';
import { useParams } from 'react-router-dom';
import Subscription from '../interfaces/Subscription.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import useSWR from 'swr';
import { useEffect, useState } from 'react';
import Row from '../interfaces/Row.interface';
import ForgeViewer from '../components/ForgeViewer';
import { Icon3dCubeSphere, IconCheck, IconCopy, IconDownload, IconScanEye, IconShare2, IconSwitchHorizontal, IconToggleLeft } from '@tabler/icons-react';
import QRCode from 'react-qr-code';
import Attachment from '../interfaces/Attachment.interface';
import { useMediaQuery } from '@mantine/hooks';

const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})

export default function ProductDetails() {
    const params = useParams();
    const [shareOpen, setShareOpen] = useState(false);
    const [mobileViewerOpen, setMobileViewerOpen] = useState(false);
    const [downloadLink, setDownloadLink] = useState<string>();
    const [activeAttachment, setActiveAttachment] = useState<Attachment>();
    const [attachment3d, setAttachment3d] = useState<Attachment>();
    const [attachment2d, setAttachment2d] = useState<Attachment>();
    const isMobile = useMediaQuery(`(max-width: ${em(750)})`);

    const { data: subscription } = useSWR<Subscription>(`https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${params.companyName}`, fetcher)
    const { data: productDefinition, isLoading } = useSWR<ProductDefinition>(subscription ? `https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${subscription.id}/product-definitions/${params.productDefinitionName}` : null, fetcher)

    const [row, setRow] = useState<Row>();

    useEffect(() => {
        if (!productDefinition) {
            setRow(undefined);
            return;
        }

        const matchingRow = productDefinition.productTable.rows.find(r => r.partNumber === params.partNumber);
        matchingRow ? setRow(matchingRow) : setRow(undefined);

        setActiveAttachment(row?.attachments.find(i => i.forgeObjectKey.includes(".stp")));
        setAttachment3d(row?.attachments.find(i => i.forgeObjectKey.includes(".stp")));
        setAttachment2d(row?.attachments.find(i => i.forgeObjectKey.includes(".pdf")));
    }, [params.partNumber, productDefinition, row?.attachments]);


    function handleDownloadChange(forgeObjectKey:string | null) {
        if (!forgeObjectKey){
            setDownloadLink(undefined);
            return;
        }

        fetch(`https://cadflairrestapi.azurewebsites.net/api/v1/forge/buckets/${productDefinition?.forgeBucketKey}/objects/${forgeObjectKey}/signed-url`)
            .then(res => res.json())
            .then(data => {
                console.log(data.url);
                setDownloadLink(data.url);
            })

    }

    function handleMobileViewerClick(attachment:Attachment) {
        setActiveAttachment(attachment);
        setMobileViewerOpen(true);
    }

    if (isLoading)
        return (
            <>
                <Stack h="100%" mih="90dvh" justify="center" align="center">
                    <Loader color="blue" />
                    <Text>Loading...</Text>
                </Stack>
            </>
        );

    return (
        <>
            {/* share dialog */}
            <Modal
                opened={shareOpen}
                onClose={() => setShareOpen(false)}
                centered
                size="lg"
                withCloseButton={false}
            >
                <Stack gap="xs" align="center">
                    <Title>Share</Title>
                    <Text>Scan the code or copy the link to share this page with a friend.</Text>
                </Stack>
                <Group justify="center" my="xl">
                    <QRCode value={window.location.href} />
                </Group>
                <TextInput value={window.location.href} my="sm" />
                <Group justify="space-between">
                    <CopyButton value={window.location.href}>
                        {({ copied, copy }) => (
                            <Button
                                leftSection={copied ? <IconCheck /> : <IconCopy />}
                                color={copied ? 'teal' : 'black'}
                                variant="outline"
                                onClick={copy}
                            >
                                {copied ? 'Copied!' : 'Copy Link'}
                            </Button>
                        )}
                    </CopyButton>
                    <Button onClick={() => setShareOpen(false)}>Done</Button>
                </Group>
            </Modal>

            <Modal
                opened={mobileViewerOpen}
                onClose={() => setMobileViewerOpen(false)}
                title={"Preview"}
                fullScreen
            >
                <Box h="90vh">
                    <ForgeViewer bucketKey={productDefinition?.forgeBucketKey} objectKey={activeAttachment?.forgeObjectKey} />
                </Box>
            </Modal>


            {/* Header */}
            <Group gap="0px" p="sm" pos="sticky" top={60} bg="#ffffff" justify="space-between" style={{ zIndex: 100 }} wrap="nowrap">
                <Stack gap="0px">
                    <Title order={2} lineClamp={1}>{productDefinition?.name}</Title>
                    <Text>Part Number: {row?.partNumber}</Text>
                </Stack>
                <ActionIcon variant="subtle" color="black" hiddenFrom="sm" onClick={() => setShareOpen(true)} ml="xs">
                    <IconShare2 />
                </ActionIcon>
                <Button
                    onClick={() => setShareOpen(true)}
                    leftSection={<IconShare2 />}
                    variant="subtle"
                    color="black"
                    visibleFrom="md"
                >
                    Share
                </Button>
            </Group>

            <Grid px="md">
                {/* viewer desktop */}
                {!isMobile &&
                    <Grid.Col span={6} visibleFrom="md">
                        <Box pos="absolute" style={{ zIndex: 10 }} p="8px">
                            {attachment3d && activeAttachment?.forgeObjectKey.includes(".stp") &&
                                <Button
                                    leftSection={<IconScanEye/> }
                                    onClick={() => setActiveAttachment(attachment2d)}
                                    radius="xl"
                                >
                                    View 2D
                                </Button>}
                            {attachment2d && activeAttachment?.forgeObjectKey.includes(".pdf") &&
                                <Button
                                    leftSection={<Icon3dCubeSphere/> }
                                    onClick={() => setActiveAttachment(attachment3d)}
                                    radius="xl"
                                >
                                    View 3D
                                </Button>}
                        </Box>
                        <ForgeViewer bucketKey={productDefinition?.forgeBucketKey} objectKey={activeAttachment?.forgeObjectKey} />
                    </Grid.Col>}

                {/* product details */}
                <Grid.Col span={{ base: 12, md: 6 }}>
                    <Stack>
                        {attachment3d &&
                            <Button
                                leftSection={<Icon3dCubeSphere />}
                                variant="filled"
                                hiddenFrom="md"
                                color="gray.6"
                                onClick={() => handleMobileViewerClick(attachment3d)}
                            >
                                View 3D
                            </Button>}
                        {attachment2d &&
                            <Button
                                leftSection={<IconScanEye />}
                                variant="filled"
                                hiddenFrom="md"
                                color="gray.6"
                                onClick={() => handleMobileViewerClick(attachment2d)}
                            >
                                View 2D
                            </Button>}
                        <Text size="sm" fw={500}>Downloads</Text>
                        <Flex gap="sm">
                            <Select style={{ flexGrow: "1" }}
                                placeholder="Select file"
                                data={row?.attachments.map(a => a.forgeObjectKey)}
                                onChange={(val) => handleDownloadChange(val)}
                            />
                            <Anchor target="_blank" href={downloadLink}>
                                <Button leftSection={<IconDownload />} variant="outline" color="black">
                                    Download
                                </Button>
                            </Anchor>
                        </Flex>
                        <Table>
                            <Table.Tbody>
                                {row?.tableValues.map(tableValue =>
                                    <Table.Tr key={tableValue.id}>
                                        <Table.Td>
                                            <Text fw="500">
                                                {productDefinition?.productTable.columns.find(c => c.id === tableValue.columnId)?.header}
                                            </Text>
                                        </Table.Td>
                                        <Table.Td>
                                            <Text>
                                                {tableValue.value}
                                            </Text>
                                        </Table.Td>
                                    </Table.Tr>
                                )}
                            </Table.Tbody>
                        </Table>
                        <Text>{productDefinition?.description}</Text>
                    </Stack>
                </Grid.Col>
            </Grid>
        </>
    );
}


