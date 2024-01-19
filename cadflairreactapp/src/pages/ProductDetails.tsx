import { Text, Group, Stack, Title, Table, Loader, Box, Select, Button, Flex, Modal, CopyButton, TextInput, Anchor } from '@mantine/core';
import { useParams } from 'react-router-dom';
import Subscription from '../interfaces/Subscription.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import useSWR from 'swr';
import { useEffect, useState } from 'react';
import Row from '../interfaces/Row.interface';
import ForgeViewer from '../components/ForgeViewer';
import { IconCheck, IconCopy, IconDownload, IconShare } from '@tabler/icons-react';
import { useDisclosure } from '@mantine/hooks';

const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})

export default function ProductDetails() {
    const params = useParams();
    const [opened, { open, close }] = useDisclosure(false);
    const [downloadLink, setDownloadLink] = useState<string>();

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
    }, [params.partNumber, productDefinition]);


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
            <Modal opened={opened} onClose={close} title={<Title>Share</Title>} centered size="lg">
                <Text>Scan the code or copy the link to share this page with a friend.</Text>
                <TextInput value="url" my="sm" />
                <Group justify="space-between">
                    <CopyButton value="https://mantine.dev">
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
                    <Button onClick={close}>Done</Button>
                </Group>
            </Modal>
            <Group gap="0px" p="sm" pos="sticky" top={60} bg="#ffffff" justify="space-between" style={{ zIndex: 100 }}>
                <Stack gap="0px">
                    <Title order={2}>{productDefinition?.name}</Title>
                    <Text>Part Number: {row?.partNumber}</Text>
                </Stack>
                <Button
                    onClick={open}
                    leftSection={<IconShare />}
                    variant="subtle"
                    color="black"
                >
                    Share
                </Button>
            </Group>

            <Group w="100%" grow align="flex-start" px="lg">
                <Box h="700px">
                    <ForgeViewer bucketKey={productDefinition?.forgeBucketKey} objectKey={row?.attachments[0].forgeObjectKey} />
                </Box>

                <Stack>
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
            </Group>
        </>
    );
}


