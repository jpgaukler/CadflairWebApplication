import { Title } from '@mantine/core';
import { useSubscription } from '../hooks/useSubscription';
import { useParams } from 'react-router-dom';
//import { useCategories } from '../hooks/useCategories';



// this is just a page for testing hooks
export default function TestHooks() {
    const params = useParams();

    const { subscription, isLoading, isError} = useSubscription(params.companyName!);
    //const { categories, categoriesLoading, isError} = useCategories(subscription.id);

    if (isError) return <div>failed to load</div>;
    if (isLoading) return <div>loading...</div>;

    return (
        <>
            <h1>Product Definitions Page</h1>
            <Title order={1}>{subscription?.companyName}</Title>
        </>
    );

}


