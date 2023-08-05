import React, { useEffect, useState } from 'react';
import { Grid, Paper, Button } from '@mui/material';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Typography from '@mui/material/Typography';
import { useDispatch, useSelector } from 'react-redux';
import Navbar from '../Navbar/Navbar';
import CssBaseline from '@mui/material/CssBaseline';
import { getInactiveSellersAction, getActiveSellersAction, approveSellerAction } from '../../Store/adminSlice';

export default function AdminPanel() {
  const dispatch = useDispatch();
  const [inactiveSellers, setInactiveSellers] = useState([]);
  const [allSellers, setAllSellers] = useState([]);

  const inactiveSellersFromStore = useSelector((state) => state.admin.inactiveSellers);
  const allSellersFromStore = useSelector((state) => state.admin.allSellers);

  const [isInitial, setIsInitial] = useState(true);
  useEffect(() => {
    setInactiveSellers(inactiveSellersFromStore);
    setAllSellers(allSellersFromStore);
  }, [inactiveSellersFromStore, allSellersFromStore]);

  const handleApprove = (id) => {
    const requestBody = {
      id: id,
      isActive: true,
    }
    verify(requestBody);
  };


  const handleDeny = (id) => {
    const requestBody = {
      id: id,
      isActive: false,
    }
    verify(requestBody);
  };

  const verify = (requestBody) => {
    dispatch(approveSellerAction(requestBody))
      .then(() => {
        setIsInitial(true);
        update(requestBody);
      })
      .catch((error) => {
        console.error("Verifying seller: ", error);
      });
  };
  
  const update = (requestBody) => {
    const updatedInactiveSellers = inactiveSellers.filter(
      (seller) => seller.id !== requestBody.id
    );
    const approvedSeller = inactiveSellers.find(
      (seller) => seller.id === requestBody.id
    );
    const updatedAllSellers = [...allSellers, approvedSeller];
    setInactiveSellers(updatedInactiveSellers);
    setAllSellers(updatedAllSellers);
  };
  useEffect(() => {
    if (!isInitial) {
      return;
    }

    const execute = () => {
      dispatch(getInactiveSellersAction());
      dispatch(getActiveSellersAction());
    };

    execute();
    setIsInitial(false);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isInitial]);

  if (!inactiveSellers) {
    return <div>Loading...</div>
  }

  return (
    <React.Fragment>
       <CssBaseline/>
      <Navbar />
      <Grid container justifyContent="center" mt={4}>
        <Grid item xs={12} sm={8} md={6} lg={6}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
          <Typography sx = {{fontSize: '2rem'}} color={'primary'} >PENDING SELLERS</Typography>
            {inactiveSellers.length === 0 ? (
          <Typography variant="h6" component="p">
            No sellers pending verification.
          </Typography>
        ) : (
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Full name</TableCell>
                  <TableCell>Username</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {inactiveSellers.map((seller) => (
                  <TableRow key={seller.id} >
                    <TableCell>{seller.name}</TableCell>
                    <TableCell>{seller.username}</TableCell>
                    <TableCell>{seller.email}</TableCell>
                    <TableCell align="right">
                      <Button
                        variant="contained"
                        color="primary"
                        className="approveButton"
                        onClick={() => handleApprove(seller.id)}
                      >
                        Approve
                      </Button>
                      <Button
                        sx={{ ml: 2 }}
                        variant="contained"
                        color="secondary"
                        onClick={() => handleDeny(seller.id)}
                      >
                        Deny
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>)}
          </Paper>
        </Grid>
      </Grid>
      <Grid container justifyContent="center" sx={{ mt: 4 }}>
        <Grid item xs={12} sm={8} md={6} lg={6}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
          <Typography sx = {{fontSize: '2rem'}} color={'primary'} component="p">ALL SELLERS</Typography>
            {allSellers.length === 0 ? (
          <Typography  variant="h6" component="p">
            No verified or denied sellers.
          </Typography>
        ) : (
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Full name</TableCell>
                  <TableCell>Username</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Approve status</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {allSellers.map((seller) => (
                  <TableRow key={seller.id}>
                    <TableCell>{seller.name}</TableCell>
                    <TableCell>{seller.username}</TableCell>
                    <TableCell>{seller.email}</TableCell>
                    <TableCell>{seller.approved ? 'APPROVED' : (seller.denied ? 'DENIED' : 'PENDING')}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>)}
          </Paper>
        </Grid>
      </Grid>
    </React.Fragment>
  );
}