import {axiosClient} from "./AxiosService";

export const NewOrder = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/new-order`, requestBody
    );
};

export const GetOrder = async (requestBody) => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-order`, requestBody
    );
};

export const GetOrders = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-orders`
    );
};

export const GetUndeliveredOrders = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-undelivered-orders`
    );
};

export const GetSellerNewOrders = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-seller-new-orders`
    );
};

export const GetSellerOldOrders = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-seller-old-orders`
    );
};

export const GetUserOrders = async () => {
    return await axiosClient.get(
        `${process.env.REACT_APP_API_ENDPOINT}/get-user-orders`
    );
};

export const CancelOrder = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/cancel-order`, requestBody
    );
};

export const AddOrderItems = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/add-order-items`, requestBody
    );
};

export const ApproveOrder = async (requestBody) => {
    return await axiosClient.put(
        `${process.env.REACT_APP_API_ENDPOINT}/approve-order`, requestBody
    );
};



